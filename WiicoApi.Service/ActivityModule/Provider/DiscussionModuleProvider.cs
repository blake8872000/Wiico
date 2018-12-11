using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService.Discussion;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.Service.ActivityModule.Provider
{
    class DiscussionModuleProvider : ModuleProvider
    {
        public DiscussionModuleProvider(ModuleParameter parameters) : base(parameters)
        {
        }

        public override void Delete()
        {
            var authService = new AuthService();
            var learningService = new LearningCircleService();
            var learningCircleInfo = learningService.GetDetailByOuterKey(_parameters.CircleKey);
            bool _isAdmin = authService.CheckFunctionAuth(learningCircleInfo.Id, Service.Utility.ParaCondition.DiscussionFunction.Manage, _parameters.MemberId);

            var _service = new DiscussionService();
            if (_isAdmin)
                //刪除主題討論活動
                _service.DeleteDiscussion(_parameters.EventId, _parameters.MemberId);
        }

        public override ModuleDetailViewModel GetDetail()
        {
            throw new NotImplementedException();
        }

        public override string GetDetailViewName()
        {
            throw new NotImplementedException();
        }

        public override ModulesListViewModel GetModulesListViewModel()
        {
            bool _isAdmin = IsAdmin(DiscussionFunction.Manage.ToString());
            var _pages = System.Convert.ToInt32(_parameters.Pages);
            var db = _uow.DbContext;
            var moduleActs = db.Activitys.Where(t => t.ModuleKey.Equals(_parameters.ModuleKey) && t.ToRoomId.Equals(_parameters.CircleKey) && t.CardisShow == true);

            if (_parameters.Rows != null)
            {
                var _rows = System.Convert.ToInt32(_parameters.Rows);
                moduleActs = moduleActs.OrderByDescending(t => t.Publish_Utc).Skip((_pages - 1) * _rows).Take(_rows);
            }
            else
            {
                moduleActs = moduleActs.OrderByDescending(t => t.Publish_Utc);
            }

            _list.DiscussionList = new List<DiscussionViewModel>();

            var _service = new DiscussionService();
            foreach (var act in moduleActs)
            {
                var _discussion = _service.GetForMem(act.OuterKey, _parameters.MemberId);
                _discussion.Rows = _parameters.Rows;
                _discussion.Pages = _parameters.Pages;
                _list.DiscussionList.Add(_discussion);
            }

            _list.IsAdminRole = _isAdmin;
            _list.ModuleKey = _parameters.ModuleKey;
            _list.LearningId = _circle.Id;
            _list.CircleKey = _parameters.CircleKey;
            _list.CircleName = _circle.Name;

            return _list;
        }

        public override string GetViewName()
        {
            throw new NotImplementedException();
        }

        public override void QueryDetail()
        {
            throw new NotImplementedException();
        }

    }
}
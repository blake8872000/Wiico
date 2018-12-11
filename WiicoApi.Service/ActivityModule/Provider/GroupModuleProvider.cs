using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Service.SignalRService;

namespace WiicoApi.Service.ActivityModule.Provider
{
    class GroupModuleProvider : ModuleProvider
    {

        private readonly GroupService groupService = new GroupService();

        public GroupModuleProvider(ModuleParameter parameters) : base(parameters)
        {
        }

        public override void Delete()
        {
            throw new NotImplementedException();
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
            bool _isAdmin = IsAdmin(Utility.ParaCondition.GroupFunctionStatus.Manage);
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

            _list.GroupList = new List<GroupListViewModel>();

            foreach (var act in moduleActs)
            {
                /* var groups = iThink.Models.ViewModels.GroupCategory.Get(DeveloperTools.ApiData.Utility.GuidToPageToken(act.OuterKey));
                 groups.Rows = _parameters.Rows;
                 groups.Pages = _parameters.Pages;*/
                var group = groupService.GetGroupListViewModel(_parameters.CircleKey, _parameters.MemberId, act.OuterKey);

                _list.GroupList.Add(group);
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

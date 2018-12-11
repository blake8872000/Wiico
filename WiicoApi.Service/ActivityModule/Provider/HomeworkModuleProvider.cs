using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.ViewModel;

namespace WiicoApi.Service.ActivityModule.Provider
{
    class HomeworkModuleProvider : ModuleProvider
    {
        private readonly HomeworkService homeworkService = new HomeworkService();

        public HomeworkModuleProvider(ModuleParameter parameters) : base(parameters)
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
            bool _isAdmin = base.IsAdmin(Utility.ParaCondition.HomeWorkState.HomeWorkFunctionStatus.Manage);
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

            _list.HomeWorkList = new List<HomeWorkViewModel>();

            var _service = homeworkService;
            foreach (var act in moduleActs)
            {
                var _homework = (_isAdmin ? _service.GetForAll(act.OuterKey) : _service.GetForMem(act.OuterKey, _parameters.MemberId));
                _homework.Rows = _parameters.Rows;
                _homework.Pages = _parameters.Pages;
                _list.HomeWorkList.Add(_homework);
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
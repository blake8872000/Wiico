using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.SignIn;

namespace WiicoApi.Service.ActivityModule.Provider
{
    class SigInModuleProvider : ModuleProvider
    {
        private readonly SignInService signInService = new SignInService();

        public SigInModuleProvider(ModuleParameter parameters) : base(parameters)
        {
        }

        public override void Delete()
        {
            var learningCircleService = new LearningCircleService();
            var leanringCircleInfo = learningCircleService.GetDetailByOuterKey(_parameters.CircleKey.ToLower());
            var authService = new AuthService();
            bool _isAdmin = authService.CheckFunctionAuth(leanringCircleInfo.Id, Service.Utility.ParaCondition.SignInFunction.Admin, _parameters.MemberId);

            var _service = new SignInService();
            if (_isAdmin)
                _service.Delete(_parameters.EventId, _parameters.MemberId);
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
            var isAdmin = IsAdmin(Utility.ParaCondition.SignInFunction.Admin);
            var param = new SignInEventParam() { MemberId = _parameters.MemberId, CircleKey = _parameters.CircleKey, NotDeleted = true, Pages = _parameters.Pages, Rows = _parameters.Rows };
            _list.SignInList = signInService.GetMutipEventData(param, isAdmin);
            // _list.SignInList= _service.GetList(param);
            _list.IsAdminRole = isAdmin;
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

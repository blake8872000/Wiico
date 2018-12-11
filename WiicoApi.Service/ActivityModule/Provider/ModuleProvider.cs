using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;

namespace WiicoApi.Service.ActivityModule.Provider
{
    abstract class ModuleProvider
    {
        protected readonly GenericUnitOfWork _uow;

        protected readonly CacheService cacheService = new CacheService();
        protected Infrastructure.DataTransferObject.ModuleParameter _parameters;
        protected Infrastructure.BusinessObject.CircleCacheData _circle;
        protected Infrastructure.ViewModel.ModulesListViewModel _list = new Infrastructure.ViewModel.ModulesListViewModel();
        protected Infrastructure.ViewModel.ModuleDetailViewModel _detail = new Infrastructure.ViewModel.ModuleDetailViewModel();

        public ModuleProvider(Infrastructure.DataTransferObject.ModuleParameter parameters)
        {

            _uow = new GenericUnitOfWork();
            _parameters = parameters;
            _circle = cacheService.GetCircle(_parameters.CircleKey);
        }

        protected bool IsAdmin(string functionKey)
        {
            var authService = new AuthService();
            bool _isAdmin = authService.CheckFunctionAuth(_circle.Id, functionKey, _parameters.MemberId);
            return _isAdmin;
        }

        public abstract Infrastructure.ViewModel.ModulesListViewModel GetModulesListViewModel();

        #region // 目前沒調用過 
        public abstract string GetViewName();

        public abstract void QueryDetail();

        public abstract Infrastructure.ViewModel.ModuleDetailViewModel GetDetail();

        public abstract string GetDetailViewName();

        public abstract void Delete();
        #endregion

    }
}

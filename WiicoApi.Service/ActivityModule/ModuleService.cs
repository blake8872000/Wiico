using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.ActivityModule
{
    public class ModuleService
    {
        private readonly GenericUnitOfWork _uow;

        public ModuleService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得某個模組功能清單
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<Infrastructure.BusinessObject.AuthModuleInfo> ModuleFunctions(int moduleId)
        {
            List<Infrastructure.BusinessObject.AuthModuleInfo> result = new List<Infrastructure.BusinessObject.AuthModuleInfo>();
            var db = _uow.DbContext;

            var query = from mf in db.ModuleFunction.Where(t => t.ModulesId.Equals(moduleId) && t.Enable.Equals(true))
                        join m in db.Modules on mf.ModulesId equals m.Id
                        select new
                        {
                            ModulesId = m.Id,
                            ModulesName = m.Name,
                            ModuleFunctionId = mf.Id,
                            ModuleFunctionName = mf.Name,
                            ModuleFunctionEnable = mf.Enable
                        };
            //權限模組資訊
            var resAuthModuleInfo = new Infrastructure.BusinessObject.AuthModuleInfo();
            resAuthModuleInfo.ModuleId = query.ToArray()[0].ModulesId;
            resAuthModuleInfo.ModuleName = query.ToArray()[0].ModulesName;
            //權限功能資訊list
            var listModuleFunctionInfo = new List<Infrastructure.ValueObject.ModuleFunctionInfo>();
            foreach (var _item in query)
            {
                //權限功能
                var resModuleFunctionInfo = new Infrastructure.ValueObject.ModuleFunctionInfo();

                resModuleFunctionInfo.FunctionId = _item.ModuleFunctionId;
                resModuleFunctionInfo.FunctionName = _item.ModuleFunctionName;
                resModuleFunctionInfo.Enable = _item.ModuleFunctionEnable;
                listModuleFunctionInfo.Add(resModuleFunctionInfo);
                resAuthModuleInfo.Functions = listModuleFunctionInfo.ToArray();
            }
            result.Add(resAuthModuleInfo);
            return result;
        }

        /// <summary>
        /// 取得某個學習圈的所有功能項目
        /// </summary>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ValueObject.ModuleFunctions> GetLearningFunction(int learningId)
        {
            var db = _uow.DbContext;

            var query = from mf in db.ModuleFunction
                        join la in db.LearningAuth on mf.Id equals la.FunctionId
                        join cmr in db.CircleMemberRoleplay on la.LearningRoleId equals cmr.RoleId
                        join lr in db.LearningRole on cmr.RoleId equals lr.Id
                        where lr.LearningId.Equals(learningId)
                        group mf.Id by mf into r
                        orderby r.Key.Id
                        select new Infrastructure.ValueObject.ModuleFunctions
                        {
                            Id = r.Key.Id,
                            Name = r.Key.Name,
                            Enable = true
                        };
            return query.ToList();
        }

        public Infrastructure.ViewModel.ModuleDetailViewModel GetActivityDetail(Infrastructure.DataTransferObject.ModuleParameter param)
        {
            var result = new Infrastructure.ViewModel.ModuleDetailViewModel();

            var moduleProvider = ModuleFactory.CreateModuleProvider(param);
            if (moduleProvider != null)
                result = moduleProvider.GetDetail();
            return result;
        }

        public Infrastructure.ViewModel.ModulesListViewModel GetModulesListViewModel(Infrastructure.DataTransferObject.ModuleParameter param)
        {
            Infrastructure.ViewModel.ModulesListViewModel result = new Infrastructure.ViewModel.ModulesListViewModel();

            var moduleProvider = ModuleFactory.CreateModuleProvider(param);
            if (moduleProvider != null)
                result = moduleProvider.GetModulesListViewModel();
            

            return result;
        }

        public bool DeleteActivity(Infrastructure.DataTransferObject.ModuleParameter param)
        {
            var result = new Infrastructure.ViewModel.ModulesListViewModel();

            try {
                var moduleProvider = ModuleFactory.CreateModuleProvider(param);
                if (moduleProvider != null)
                    moduleProvider.Delete();
                return true;
            } catch (Exception ex) {
                var errorService = new ErrorService();
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, "刪除活動發生錯誤"+ex.Message.ToString());
                return false;
            }
            
        }
    }
}

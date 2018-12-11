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
    class MaterialModuleProvider : ModuleProvider
    {
        private readonly MaterialService materialService = new MaterialService();

        public MaterialModuleProvider(ModuleParameter parameters) : base(parameters)
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
            var db = _uow.DbContext;
            var _pages = System.Convert.ToInt32(_parameters.Pages);
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


            _list.MaterialList = new List<MaterialViewModel>();

            foreach (var act in moduleActs)
            {
                var materia = materialService.GetFileInfoListForDB(act.OuterKey);
                //materia.Rows = Convert.ToInt32(_parameters.Rows);
                //materia.Pages = Convert.ToInt32(_parameters.Pages);
                _list.MaterialList.Add(materia);
            }

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

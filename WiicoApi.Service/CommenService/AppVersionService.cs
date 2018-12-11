using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class AppVersionService
    {
        private readonly GenericUnitOfWork _uow;

        public AppVersionService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得版號資訊
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public AppVersionViewModel GetData(string requestData)
        {
            var db = _uow.DbContext;

            var data = db.AppVersion.FirstOrDefault(t => t.AppSystem.ToLower() == requestData.ToLower());
            if (data == null)
                return null;

            var result = new AppVersionViewModel()
            {
                CreationTime = data.CreateUtcDate.ToLocalTime(),
                Length = 0,
                Name = data.AppSystem,
                Version = data.Version
            };
            if (data.UpdateUtcDate.HasValue)
                result.CreationTime = data.UpdateUtcDate.Value.ToLocalTime();
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api
{
    /// <summary>
    /// 取得版本資訊
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class CheckUpdateController : ApiController
    {
        /// <summary>
        /// 取得版本資訊
        /// </summary>
        /// <param name="strAccess">裝置系統 - IOS | Android</param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess) {

            var service = new AppVersionService();
            var result = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.AppVersionViewModel>();
            var data = service.GetData(strAccess);
            if (data == null)
            {
                result.Success = false;
                result.Message = "查詢失敗";
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok( result);
            }
            result.Success = true;
            result.Message = "查詢成功";
            
            result.Data =new Infrastructure.ViewModel.AppVersionViewModel[1] { data };
            return Ok(result);
        }
    }
}

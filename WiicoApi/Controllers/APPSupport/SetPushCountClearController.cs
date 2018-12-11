using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    ///  
    /// </summary>
    [EnableCors("*","*","*")]
    public class SetPushCountClearController : ApiController
    {
        /// <summary>
        /// 修改badge為0
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess) {
            var requestService = new Service.Utility.RequestDataHelper<BackendBaseRequest>();
            var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
            var checkColumKey = new string[2] { "account", "icantoken" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = false;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var responseData = tokenService.SetPushCountClear(requestData.ICanToken.ToLower());
            if (responseData) {
                response.Success = true;
                response.Message = "修改成功";
                return Ok(response);
            }
            response.Message = "修改失敗";
            return Ok(response);
        }
    }
}

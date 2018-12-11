using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{/// <summary>
/// 修改pushToken資料
/// </summary>
    [EnableCors("*", "*", "*")]
    public class DevicePushTokenController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        [HttpPut]
        public IHttpActionResult Put(Infrastructure.ViewModel.Login.LoginRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Login.LoginRequest>();
            var checkColumnKey = new string[2] { "account", "pushtoken" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            if (checkEmpty == false)
            {
                response.Success = false;
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var responseMsg = tokenService.UpdateMemberPushToken(requestData.Account, requestData.PushToken);
            if (responseMsg == false)
            {
                response.Success = false;
                response.Message = "修改失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }

            response.Success = true;
            response.Message = "修改成功";

            return Ok(response);
        }
    }
}

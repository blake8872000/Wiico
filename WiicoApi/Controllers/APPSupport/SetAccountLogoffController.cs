using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 登出API
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SetAccountLogoffController : ApiController
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<string>();
            if (requestData.Account == null ||
                requestData.ICanToken == null)
            {
                response.Success = false;
                response.Message = "遺漏資訊";
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var logoutService = new LoginService();
            var logoutscuuess = logoutService.LogOut(requestData.ICanToken);
            if(logoutscuuess==false)
            {
                response.Success = false;
                response.Message = "登出失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }

            FormsAuthentication.SignOut();
            response.Message = "登出成功";
            response.Success = true;
            return Ok(response);
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody] BackendBaseRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<string>();

            if (requestData.Account == null ||
                requestData.ICanToken == null)
            {
                response.Success = false;
                response.Message = "遺漏資訊";
                return Content(HttpStatusCode.BadRequest, response);
            }
            var logoutService = new LoginService();
            var logoutscuuess = logoutService.LogOut(requestData.ICanToken);
            if (logoutscuuess == false)
            {
                response.Success = false;
                response.Message = "登出失敗";
                return Ok(response);
            }
            FormsAuthentication.SignOut();
            response.Message = "登出成功";
            response.Success = true;
            return Ok(response);
        }
    }
}

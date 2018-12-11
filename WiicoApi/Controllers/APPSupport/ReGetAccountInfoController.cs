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
    /// 重新取得帳號資訊
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class ReGetAccountInfoController : ApiController
    {
        /// <summary>
        /// 重新取得
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess) {
            var content = new ResultBaseModel<Infrastructure.ViewModel.Login.LoginResponse>();
            try
            {
                var service = new LoginService();
                var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
                var tokenService = new TokenService();
                var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
                if (checkToken == null)
                {
                    content.Success = false;
                    content.Message = "已登出";
                    content.State = Infrastructure.ViewModel.Base.LogState.Logout;
                    return Ok(content);
                }
                var result = service.ReGetAccountInfo(requestData);

                if (result != null)
                {
                    content.Success = true;
                    content.Data = new Infrastructure.ViewModel.Login.LoginResponse[1] { result };
                    content.Message = "登入成功";
                    return Ok(content);
                }
                else
                {
                    content.Success = false;
                    content.Data = new Infrastructure.ViewModel.Login.LoginResponse[0];
                    content.Message = "登入失敗";
                    return Ok(content);
                }
            }
            catch (Exception ex)
            {
                content.Success = false;
                content.Data = new Infrastructure.ViewModel.Login.LoginResponse[0];
                content.Message = ex.Message;
                return Content(HttpStatusCode.Forbidden, content);
            }
        }
    }
}

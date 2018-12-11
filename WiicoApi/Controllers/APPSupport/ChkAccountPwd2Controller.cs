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
    /// 驗證使用者登入
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class ChkAccountPwd2Controller : ApiController
    {
        /// <summary>
        /// 登入 - 驗證帳號密碼
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        //public IHttpActionResult Get([FromUri]string strAccess) {
        [EnableCors("*","*","*")]
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var content = new ResultBaseModel<Infrastructure.ViewModel.Login.LoginResponse>();
            try
            {
                var service = new LoginService();
                var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Login.LoginRequest>(strAccess);
                requestData.PhoneID = requestData.PhoneID.Replace("-", "").ToLower();

                var result = service.LoginProxy(requestData);

                if (result != null)
                {
                    content.Success = true;
                    content.Data = new Infrastructure.ViewModel.Login.LoginResponse[1] { result };
                    content.Message = "登入成功";
                    content.State = LogState.Suscess;
                    return Ok(content);
                }
                else
                {
                    content.Success = false;
                    content.Data = new Infrastructure.ViewModel.Login.LoginResponse[0];
                    content.Message = "登入失敗";
                    content.State = LogState.NoAccount;
                    return Ok( content);
                }
            }
            catch (Exception ex)
            {
                content.Success = false;
                content.Data = new Infrastructure.ViewModel.Login.LoginResponse[0];
                content.Message = ex.Message;
                content.State = LogState.Error;
                return Content(HttpStatusCode.Forbidden, content);
            }
        }

        /// <summary>
        /// 驗證使用者登入
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Post(Infrastructure.ViewModel.Login.LoginRequest requestData) {

            var content = new ResultBaseModel<Infrastructure.ViewModel.Login.LoginResponse>();
            try
            {
                var service = new LoginService();
            //    var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Login.LoginRequest>(strAccess);
                requestData.PhoneID = requestData.PhoneID.Replace("-", "").ToLower();

                var result = service.LoginProxy(requestData);

                if (result != null)
                {
                    content.Success = true;
                    content.Data = new Infrastructure.ViewModel.Login.LoginResponse[1] { result };
                    content.Message = "登入成功";
                    content.State = LogState.Suscess;
                    return Ok(content);
                }
                else
                {
                    content.Success = false;
                    content.Data = new Infrastructure.ViewModel.Login.LoginResponse[0];
                    content.Message = "登入失敗";
                    content.State = LogState.NoAccount;
                    return Ok(content);
                }
            }
            catch (Exception ex)
            {
                content.Success = false;
                content.Data = new Infrastructure.ViewModel.Login.LoginResponse[0];
                content.Message = ex.Message;
                content.State = LogState.Error;
                return Content(HttpStatusCode.Forbidden, content);
            }
        }
    }
}

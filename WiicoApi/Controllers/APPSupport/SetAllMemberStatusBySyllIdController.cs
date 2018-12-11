using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SchoolApi;

namespace WiicoApi.Controllers.api.APPSupport
{
    [EnableCors("*", "*", "*")]
    public class SetAllMemberStatusBySyllIdController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Infrastructure.ViewModel.School.SignInSynchronize.SignSyncClientRequest requestData) {
      //      var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.School.SignInSynchronize.SignSyncClientRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.School.SignInSynchronize.SignSyncClientRequest>();
            var checkColumnKeys = new string[4] { "icantoken", "classid", "syll_id", "times" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<string>();
            response.Success = false;
            response.Data = new string [0];
            if (checkEmpty == false) {
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest,response);
            }
            requestData.Token = requestData.ICanToken;
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null) {
                response.Data = new string[0];
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Content(HttpStatusCode.Forbidden, response);
            }
    
            var signInSyncService = new SignInSyncService();
            var responseData = signInSyncService.SyncToiCan(requestData);
            if (responseData == Infrastructure.ViewModel.Base.LogState.Suscess)
            {
                response.Success = true;
                response.Data = new string[1] {"同步成功" };
                response.Message = "同步成功";
            }
            else {
                response.Message = responseData.ToString();
                response.State = responseData;
            }
            return Ok(response);
        }
    }
}

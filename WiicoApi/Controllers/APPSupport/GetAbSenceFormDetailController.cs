using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.Leave;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得單一請假單資料
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetAbSenceFormDetailController : ApiController
    {
        public IHttpActionResult Get([FromUri]string strAccess) {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.ActivityFunction.Leave.GetAbSenceFormDetailRequest>(strAccess);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveData>();
            if (requestData.Account == null ||
                requestData.ClassID == null ||
                requestData.ICanToken == null ||
                requestData.OuterKey == null
                )
            {
                response.Success = false;
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }

            var service = new LeaveService();
            var data = service.GetAbSenceFormDetail(requestData);
            if (data != null)
                response.Data = data ;
            else {
                response.Success = false;
                response.Message = "查無資料";
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            return Ok(response);
        }

    }
}

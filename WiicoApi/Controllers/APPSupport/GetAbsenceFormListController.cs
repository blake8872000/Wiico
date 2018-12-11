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
    /// 請假查詢
    /// </summary>
    [EnableCors("*","*", "*")]
    public class GetAbsenceFormListController : ApiController
    {
        /// <summary>
        /// 取得請假列表
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess) {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.ActivityFunction.Leave.GetAbsenceFormListRequest>(strAccess);
            if (requestData.Account == null ||
                requestData.ClassID == null ||
                requestData.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");



            var service = new LeaveService();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.ViewModel.ActivityFunction.Leave.GetAbsenceFormListResponse>();
            
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }

            var data = service.GetAbsenceFormList(requestData);
            if (data == null)
            {
                response.Message = "無假單資料";
                response.Success = false;
                response.Data = new Infrastructure.ViewModel.ActivityFunction.Leave.GetAbsenceFormListResponse() ;
                return Ok(response);
            }
            response.State = Infrastructure.ViewModel.Base.LogState.Suscess;
            response.Message = "查詢成功";
            response.Success = true;
            response.Data = data;
            return Ok(response);
        }
    }
}

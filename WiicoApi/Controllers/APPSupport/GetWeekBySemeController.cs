using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得上課時間控制器
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetWeekBySemeController : ApiController
    {
        public IHttpActionResult Get([FromUri]string strAccess) {
            var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
            if (requestData.Account == null ||
                requestData.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");
            var service = new TimeTableService();
             var response = new ResultBaseModel<Infrastructure.ViewModel.CourseManage.GetWeekBySemeResponse>();
           var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }
            var data = service.APPGetWeekBySeme(requestData.ICanToken);

            if (data == null)
            {
                response.Success = false;
                response.Data = new Infrastructure.ViewModel.CourseManage.GetWeekBySemeResponse[0];

                response.Message = "查無資料";
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = data.ToArray();
            return Ok(response);
        }
    }
}

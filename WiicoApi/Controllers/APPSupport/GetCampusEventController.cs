using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得本學期大小事件清單
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetCampusEventController : ApiController
    {
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<GetAllMyCourseRequest>(strAccess);
            if (requestData.Account == null ||
                requestData.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");

            requestData.Token = requestData.ICanToken;
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.School.GetCampusEventGetResponse>();
            response.Success = false;
            var calendarService = new CalendarService();
            var responseData = calendarService.GetList(requestData);
            if (responseData != null)
            {
                response.Success = true;
                response.Message = "查詢成功";
                response.Data = responseData.ToArray();
            }
            else
            {
                response.Success = true;
                response.Data = new Infrastructure.ViewModel.School.GetCampusEventGetResponse[0];
                response.Message = "查無資料";
            }
            return Ok(response);

        }
    }
}

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
    /// 取得學習圈詳細資訊
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetCourseDetailController : ApiController
    {
        public IHttpActionResult Get([FromUri]string strAccess) {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.CourseManage.GetCourseDetailRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.CourseManage.GetCourseDetailRequest>();
            var checkColumnKeys = new string[3] { "account", "classid", "icantoken" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkEmpty==false)
                return Content(HttpStatusCode.InternalServerError, "遺漏參數");
            var response = new ResultBaseModel<Infrastructure.ViewModel.CourseManage.GetCourseDetailResponse>();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }


            var service = new LearningCircleService();
            var data = service.APPGetCourseDetail(requestData.ICanToken, requestData.ClassID);
         
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = new Infrastructure.ViewModel.CourseManage.GetCourseDetailResponse[1] { data };

            return Ok(response);
        }
    }
}

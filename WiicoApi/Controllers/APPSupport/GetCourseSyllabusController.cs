using WiicoApi.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得課程進度控制器
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetCourseSyllabusController : ApiController
    {
 
        public IHttpActionResult Get([FromUri] string strAccess) {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.CourseManage.GetCourseSyllabusRequest>(strAccess);
            if (requestData.ClassID == null ||
                      requestData.Account == null ||
                      requestData.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");
            var service = new SyllabusService();

            var result = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.CourseManage.GetCourseSyllabusResponse>();

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;

            if (checkToken == null)
            {
                result.Success = false;
                result.Message = "已登出";
                result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Content(HttpStatusCode.BadRequest, result);
            }
            var data = service.APPGetCourSyllabus(requestData.ICanToken,requestData.ClassID);
            if (data == null)
            {
                result.Success = false;
                result.Message = "查詢錯誤";
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(result);
            }
            result.Success = true;
            result.Data = data.ToArray();
            result.Message = "查詢成功";
            return Ok(result);
        }
        /// <summary>
        /// 取得課程進度列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Post([FromBody] Infrastructure.ViewModel.CourseManage.GetCourseSyllabusRequest value) {

            if (value.ClassID == null ||
                value.Account == null ||
                value.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");

            var service = new SyllabusService();
            var data = service.APPGetCourSyllabus(value.ICanToken,value.ClassID);
            var result = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.CourseManage.GetCourseSyllabusResponse>();
            if (data == null)
            {
                result.Success = false;
                result.Message = "查無資料";
                return Ok(result);
            }
            result.Success = true;
            result.Data = data.ToArray();
            result.Message = "查詢成功";
            return Ok(result);
        }
    }
}

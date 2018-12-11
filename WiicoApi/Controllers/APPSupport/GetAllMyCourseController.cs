
using WiicoApi.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;
using WiicoApi.Infrastructure.ViewModel.Base;
using System.Web;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得課程列表
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetAllMyCourseController : ApiController
    {
        /// <summary>
        /// 取得登入者學習圈資訊
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>

        public IHttpActionResult Get([FromUri]string strAccess) {
            strAccess = HttpUtility.UrlDecode(strAccess);
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.CourseManage.GetAllMyCourseRequest>(strAccess);
            if (requestData.Account == null ||
                requestData.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");

            var result = new ResultBaseModel<Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse>();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                result.Success = false;
                result.Message = "已登出";
                result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(result);
            }
            var service = new LearningCircleService();
            var response = service.APPGetAllMyCourse(requestData.ICanToken);
            if (response == null)
            {
                result.Success = false;
                result.Message = "查無資料";
                return Ok(result);
            }
            result.Success = true;
            result.Message = "查詢成功";
            if (response.FirstOrDefault() != null)
                result.Data = response.ToArray();
            else
                result.Data = new Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse[0];
            return Ok(result);
        }
        /// <summary>
        /// 取得登入者學習圈資訊
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Post([FromBody]Infrastructure.ViewModel.CourseManage.GetAllMyCourseRequest value) {
            if (value.Account == null ||
                value.ICanToken == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");

            var result = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse>();
            var service = new LearningCircleService();
            var response = service.APPGetAllMyCourse(value.ICanToken);
            if (response == null)
            {
                result.Success = false;
                result.Message = "查詢失敗";
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            result.Success = true;
            result.Message = "查詢成功";
            result.Data = response.ToArray();
            return Ok(result);
        }
    }
}

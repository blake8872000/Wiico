using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 課程權限
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class CourseManageAuthController : ApiController
    {
        /// <summary>
        /// 課程管理權限查詢
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<BackendBaseRequest>();
            var checkColumns = new string[2] { "token", "circlekey" };
            //var getHeadToken = ActionContext.Request.Headers.GetValues("X-Token");
            var checkEmptData = requestService.CheckColumnEmpty(requestData, checkColumns);
            var response = new BaseResponse<CourseManageAuthResponse>();
            response.Success = false;
            response.Data = new CourseManageAuthResponse()
            {
                CircleAdminSetting = new CircleAdminSettingAuth() { Admin = false, DeleteCircleAdmin = false },
                CircleInfoSetting = new CircleInfoSettingAuth() { Admin = false },
                CircleMemberSetting = new CircleMemberSettingAuth() { Admin = false, AddCircleMember = false, DeleteCircleMember = false },
                CircleRoleSetting = new CircleRoleSettingAuth() { Admin = false, AddCircleRole = false, DeleteCircleRole = false },
                CircleScheduleSetting = new CircleScheduleSettingAuth() { Admin = false },
                CircleTimelistSetting = new CircleTimelistSettingAuth() { Admin = false }
            };
            if (checkEmptData == false)
            {
                response.Success = false;
                response.Message = "遺漏參數";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var authService = new AuthService();
            var responseData = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            response.Success = true;
            if (responseData == null)
            {
                response.Message = "無權限";
                return Ok(response);
            }
            response.Message = "查詢成功";
            response.Data = responseData;
            return Ok(response);
        }
    }
}

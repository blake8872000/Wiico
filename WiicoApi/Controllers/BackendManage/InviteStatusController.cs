using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.Utility;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 邀請碼啟用狀態
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class InviteStatusController : ApiController
    {
        private MemberInviteService memberInviteService = new MemberInviteService();
        /// <summary>
        /// 取得課程的邀請開關
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<MemberInvitePostRequest>(strAccess);
            var requestService = new RequestDataHelper<MemberInvitePostRequest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "invitetype" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<bool>();
            response.Success = false;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = memberInviteService.GetLearningInviteStatus(requestData.Token, requestData.CircleKey.ToLower(), requestData.InviteType);
            response.Data = responseData;
            response.Success = true;
            if (responseData)
                response.Message = "啟用邀請";
            else
                response.Message = "停用邀請";
            return Ok(response);
        }

        /// <summary>
        /// 變更課程的邀請開關
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPut, TokenValidation]
        public IHttpActionResult Put([FromBody] MemberInvitePostRequest requestData)
        {
            var requestService = new RequestDataHelper<MemberInvitePostRequest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "invitetype" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<bool>();
            response.Success = false;

            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            var authService = new AuthService();
            var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            //除了課程管理者可以修改邀請開關，其他角色都不得設定
            if (checkManageAuth == null || checkManageAuth.CircleMemberSetting.AddCircleMember == false)
            {
                response.Message = "無權限邀請";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            var responseData = memberInviteService.UpdateLearningInviteStatus(requestData.Token, requestData.CircleKey.ToLower(), requestData.InviteType);
            response.Data = responseData;
            response.Success = true;
            if (responseData)
                response.Message = "啟用邀請";
            else
                response.Message = "停用邀請";
            return Ok(response);
        }
    }
}

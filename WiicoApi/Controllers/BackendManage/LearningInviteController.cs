using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Service.Backend;
using WiicoApi.Service.Utility;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 學習圈邀請
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class LearningInviteController : ApiController
    {
        private MemberInviteService memberInviteService = new MemberInviteService();


        /// <summary>
        /// 取得邀請碼資訊
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<MemberInviteGetRequest>(strAccess);
            var requestService = new RequestDataHelper<MemberInviteGetRequest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "ismaincode" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<IEnumerable<Infrastructure.Entity.MemberInvite>>();
            response.Success = false;
            response.Data = new List<Infrastructure.Entity.MemberInvite>();
            if (checkEmpty == false)
            {
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Message = "遺漏參數";
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = memberInviteService.GetList(requestData.CircleKey.ToLower(), 0, null);
            if (requestData.IsMainCode)
                responseData = responseData.Where(t => t.Enable == true && t.IsCourseCode == true);
            if (responseData == null)
            {
                response.Message = "查無資訊";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            else
            {
                response.Success = true;
                response.Message = "查詢成功";
                response.Data = responseData;
            }
            return Ok(response);
        }
        /// <summary>
        /// 建立邀請碼
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Post(MemberInvitePostRequest requestData)
        {
            var requestService = new RequestDataHelper<MemberInvitePostRequest>();
            var checkColumnKey = new string[4] { "token", "circlekey", "roletype", "inviteemail" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<bool>();
            response.Success = false;

            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = memberInviteService.Create(requestData);
            response.Data = responseData;
            if (responseData)
            {
                response.Success = true;
                response.Message = "建立成功";
            }
            else
            {
                response.Message = "建立失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            return Ok(response);
        }
        [TokenValidation]
        public IHttpActionResult Put(MemberInvitePutRequest requestData)
        {
            var requestService = new RequestDataHelper<MemberInvitePutRequest>();
            var checkColumnKey = new string[1] { "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<MemberInvite>>();
            response.Success = false;
            response.Data = new List<MemberInvite>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = memberInviteService.Update(requestData);
            if (responseData != null)
            {
                response.Success = true;
                response.Message = "更新成功";
                response.Data = responseData.ToList();
            }
            else
            {
                response.Message = "更新失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            return Ok(response);
        }

        public IHttpActionResult Delete() { return Ok(); }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Infrastructure.ViewModel.MemberManage;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 課程成員角色管理
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class CircleMemberRoleManageController : ApiController
    {
        private CircleMemberService circleMemberService = new CircleMemberService();
        /// <summary>
        /// 取得資訊
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);

            if (requestData.CircleKey == null || requestData.CircleKey == string.Empty)
                return Content(HttpStatusCode.BadRequest, "遺漏資訊");

            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<Infrastructure.ViewModel.MemberManage.CircleMemberRoleManageGetResponse>>();
            //     var responseData = circleMemberService.GetCircleMemberRoleListByCircleKey(requestData.CircleKey);
            var responseData = circleMemberService.GetAzureCircleMemberRoleListByCircleKey(requestData.CircleKey, requestData.Token);
            response.Success = false;
            response.Data = new List<Infrastructure.ViewModel.MemberManage.CircleMemberRoleManageGetResponse>();
            if (responseData == null)
            {
                response.State = Infrastructure.ViewModel.Base.LogState.Error;

                response.Message = "查詢失敗";
                return Ok(response);
            }
            //   result.Data = new Infrastructure.ViewModel.Backend.CircleMemberRoleListViewModel();
            response.Data = responseData.ToList();
            response.Success = true;
            response.Message = "查詢成功";
            return Ok(response);
        }

        /// <summary>
        /// 建立成員角色資料
        /// </summary>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Post(CircleMemberRoleRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();

            var requestService = new Service.Utility.RequestDataHelper<CircleMemberRoleRequest>();
            var checkColumnKey = new string[1] { "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            response.Success = false;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            //後臺新增
            if (requestData.InviteCode == null && requestData.RoleId.HasValue)
            {
                var authService = new AuthService();
                var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
                //除了課程管理者可以修改，其他角色都不得修改
                if (checkManageAuth == null || checkManageAuth.CircleMemberSetting.AddCircleMember == false)
                {
                    response.Message = "無法加入";
                    response.State = Infrastructure.ViewModel.Base.LogState.Error;
                    return Ok(response);
                }

                var proxyReponse = circleMemberService.InsertMutipleCircleMemberRole(requestData);
                response.Success = proxyReponse;
                if (proxyReponse)
                {
                    response.Message = "新增成功";
                    return Ok(response);
                }
                else
                {
                    response.State = Infrastructure.ViewModel.Base.LogState.Error;
                    response.Message = "新增失敗";
                    return Ok(response);
                }
            }
            else
            { //邀請碼新增

                var inviteService = new MemberInviteService();
                var inviteInfo = inviteService.GetDetail(requestData.InviteCode.ToLower());

                if (inviteInfo == null)
                {
                    response.Message = "無法加入";
                    response.State = Infrastructure.ViewModel.Base.LogState.Error;
                    return Ok(response);
                }
                var inviteResponse = new Infrastructure.ViewModel.Base.BaseResponse<InviteResponseData>();
                var organizationService = new OrganizationService();
                var checkCanRegister = organizationService.CheckCanRegister(requestData.OrgCode);
                if (checkCanRegister == false)
                {
                    inviteResponse.Message = "不允許加入課程";
                    inviteResponse.State = Infrastructure.ViewModel.Base.LogState.Error;
                    inviteResponse.Data = new InviteResponseData() { InviteStatus = InviteStatusEnum.EndInvite, IsOrgRegister = false, CircleKey = requestData.CircleKey };
                    return Ok(inviteResponse);
                }

                var responseData = circleMemberService.InsertCircleMemberRoleByInvite(requestData);

                inviteResponse.Success = true;
                inviteResponse.Data = responseData;
                inviteResponse.Message = responseData.InviteStatus.ToString();
                return Ok(inviteResponse);
            }
        }
        /// <summary>
        /// 編輯成員角色資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [TokenValidation, HttpPut]
        public IHttpActionResult Put(CircleMemberRolePutRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<IEnumerable<CircleMemberRoleManageGetResponse>>();
            var requestService = new Service.Utility.RequestDataHelper<CircleMemberRolePutRequest>();
            var checkColumnKey = new string[4] { "token", "circlekey", "accounts", "roleid" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            response.Success = false;
            response.Data = new List<CircleMemberRoleManageGetResponse>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var authService = new AuthService();
            var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            //除了課程管理者與老師可以修改，其他角色都不得修改
            if (checkManageAuth == null || checkManageAuth.CircleMemberSetting.Admin == false)
            {
                response.Message = "無權限修改";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            //如果是課程管理者，就可以做任何事(嘿嘿嘿...
            var responseData = checkManageAuth.CircleMemberSetting.AddCircleMember == true ?
                circleMemberService.UpdateMutiple(requestData, true) :
                circleMemberService.UpdateMutiple(requestData);
            if (responseData)
            {
                response.Success = true;
                response.Message = "修改成功";
                var datas = circleMemberService.GetAzureCircleMemberRoleListByCircleKey(requestData.CircleKey.ToLower(), requestData.Token);
                if (datas.FirstOrDefault() != null)
                    response.Data = datas;
            }
            else
            {
                response.Message = "修改失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// 刪除成員角色資料
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Delete(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<CircleMemberRolePutRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<CircleMemberRolePutRequest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "accounts" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<bool>();
            response.Success = false;
            response.Data = false;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var authService = new AuthService();
            var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            //除了課程管理者與老師可以修改，其他角色都不得修改
            if (checkManageAuth == null || checkManageAuth.CircleMemberSetting.Admin == false)
            {
                response.Message = "無權限刪除";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            var proxyReponse = circleMemberService.DeleteCircleMemberRole(requestData);
            response.Success = proxyReponse;
            response.Data = proxyReponse;
            if (proxyReponse)
                response.Message = "刪除成功";
            else
            {
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "刪除失敗";
            }
            return Ok(response);
        }
    }
}

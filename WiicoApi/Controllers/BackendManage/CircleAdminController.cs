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
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Service.Backend;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 課程管理
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class CircleAdminController : ApiController
    {
        private readonly CourseManagerService courseManagerService = new CourseManagerService();
        private Repository.GenericUnitOfWork _uow ; 
        /// <summary>
        /// 初始化
        /// </summary>
        public CircleAdminController(Repository.GenericUnitOfWork uow) {
            _uow = uow ;
            courseManagerService = new CourseManagerService(_uow);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public CircleAdminController() {
            _uow = new Repository.GenericUnitOfWork();
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Base.BackendBaseRequest>();
            var checkColumnKey = new string[2] { "circlekey", "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<Infrastructure.ViewModel.CourseManage.CourseManagerGetResponse>>();
            response.Success = false;
            response.Data = new List<Infrastructure.ViewModel.CourseManage.CourseManagerGetResponse>();
            if (checkEmpty == false)
            {
                response.Message = "請確認是否遺漏";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = courseManagerService.GetManagers(requestData.Token, requestData.CircleKey);
            if (responseData == null)
            {
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "無課程管理者";
                return Ok(response);
            }
            response.Data = responseData;
            response.Message = "查詢成功";
            response.Success = true;
            return Ok(response);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(CourseManagerPostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<CourseManagerPostRequest>();
            var checkColumnKey = new string[4] { "token", "invitecode", "restype", "accounts" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<InviteResponseData>();
            response.Success = false;

            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                return Content(HttpStatusCode.BadRequest, response);
            }
            var organizationService = new OrganizationService();
            var checkCanRegister = organizationService.CheckCanRegister(requestData.OrgCode);
            if (checkCanRegister == false)
            {
                response.Message = "不允許加入課程";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Data = new InviteResponseData() { InviteStatus = InviteStatusEnum.EndInvite, IsOrgRegister = false, CircleKey = requestData.CircleKey };
                return Ok(response);
            }
            var responseData = courseManagerService.CreateMutiple(requestData);
            response.Success = true;
            response.Message = responseData.InviteStatus.ToString();

            response.Data = responseData;
            return Ok(response);
        }
        


        public IHttpActionResult Put()
        {
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult Delete(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.CourseManage.CourseManagerDeleteRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.CourseManage.CourseManagerDeleteRequest>();
            var checkColumnKey = new string[2] { "circlekey", "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<bool>();
            response.Success = false;
            response.Data = false;
            if (checkEmpty == false)
            {
                response.Message = "請確認是否遺漏";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = courseManagerService.DeleteMutiple(requestData);
            response.Data = responseData;
            if (responseData)
            {
                response.Success = true;
                response.Message = "刪除成功";
            }
            else
            {
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "刪除失敗";
            }
            return Ok(response);
        }
    }
}

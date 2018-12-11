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
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Service.Backend;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 組織角色管理
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class OrganizationRoleController : ApiController
    {
        /// <summary>
        /// 取得組織角色列表
        /// </summary>
        /// <param name="strAccess">orgCode:組織代碼 | search:查詢條件(編號、名稱、角色代碼) | token:查詢者代碼</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<OrganizationRoleGetRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<OrganizationRoleGetRequest>();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<OrganizationRole>>();
            var checkColumnKeys = new string[2] { "token", "orgcode" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var organizationRoleService = new OrganizationRoleService();
            var responseData = organizationRoleService.GetListByRequest(requestData);
            if (responseData == null)
            {
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Success = false;
                response.Message = "查詢失敗";
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = responseData;
            return Ok(response);
        }
        /// <summary>
        /// 設定處理組織角色控制器
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost, TokenValidation]
        public IHttpActionResult Post(OrganizationRolePostRequest requestData)
        {

            var requestService = new Service.Utility.RequestDataHelper<OrganizationRolePostRequest>();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<OrganizationRole>>();
            var checkColumnKeys = new string[3] { "token", "orgcode", "orgRoles" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var organizationRoleService = new OrganizationRoleService();
            var responseData = organizationRoleService.PostByRequest(requestData);
            if (responseData == null)
            {
                response.Success = false;
                response.Message = "處理失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            response.Success = true;
            response.Message = "處理成功";
            response.Data = responseData;
            return Ok(response);
        }
    }
}

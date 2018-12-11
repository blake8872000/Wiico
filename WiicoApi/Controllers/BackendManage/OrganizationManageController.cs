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
    /// 組織管理
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class OrganizationManageController : ApiController
    {  /// <summary>
       /// 查詢組織
       /// </summary>
       /// <param name="strAccess"></param>
       /// <param name="token"></param>
       /// <param name="search"></param>
       /// <param name="orgId"></param>
       /// <param name="page"></param>
       /// <param name="row"></param>
       /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var orgService = new OrganizationService();
            var requestData = JsonConvert.DeserializeObject<OrganizationGetRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<OrganizationGetRequest>();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<Organization>>();
            var checkColumnKeys = new string[1] { "token" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            int orgId = 0;
            Int32.TryParse(requestData.Search, out orgId);
            var responseData = orgId > 0 ? orgService.GetOrganization(requestData.Token, requestData.Search, orgId) : orgService.GetOrganization(requestData.Token, requestData.Search, null);

            if (responseData == null)
            {
                response.Message = "查詢失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Success = false;
                return Ok(response);
            }
            response.Data = responseData.ToList();
            response.Success = true;
            response.Message = "查詢成功";
            return Ok(response);
        }

        /// <summary>
        /// 建立組織
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(OrganizationPostRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<Organization>();
            var requestService = new Service.Utility.RequestDataHelper<OrganizationPostRequest>();
            var checkColumnKeys = new string[3] { "name", "orgcode", "token" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var orgService = new OrganizationService();
            var responseData = orgService.CreateOrganization(requestData);
            if (responseData == null)
            {
                response.Message = "建立失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Success = false;
                return Ok(response);
            }
            response.Data = responseData;
            response.Success = true;
            response.Message = "建立成功";
            return Ok(response);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [TokenValidation, HttpPut]
        public IHttpActionResult Put(OrganizationPutRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<Organization>();
            var requestService = new Service.Utility.RequestDataHelper<OrganizationPutRequest>();
            var checkColumnKeys = new string[3] { "id", "orgcode", "token" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var orgService = new OrganizationService();
            var responseData = orgService.UpdateOrganization(requestData);
            if (responseData == null)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "編輯失敗";
                return Ok(response);
            }
            response.Success = true;
            response.Message = "編輯成功";
            response.Data = responseData;
            return Ok(response);
        }

        /// <summary>
        /// 刪除組織
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult Delete(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<OrganizationDeleteRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<OrganizationDeleteRequest>();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<bool>();
            var checkColumnKeys = new string[2] { "token", "id" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var orgService = new OrganizationService();
            var responseData = orgService.DeleteOrganization(requestData);
            response.Data = responseData;
            if (responseData == false)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "刪除失敗";
                return Ok(response);
            }
            response.Success = true;
            response.Message = "刪除成功";
            return Ok(response);
        }
    }
}

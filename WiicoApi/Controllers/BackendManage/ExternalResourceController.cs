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
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.Backend;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 同步API控制器
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class ExternalResourceController : ApiController
    {
        /// <summary>
        /// 取得API資訊列表
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<BackendBaseRequest>();

            var response = new ResultBaseModel<ExternalResource>();
            response.Success = false;
            response.Data = new ExternalResource[0];
            var checkColumnKeys = new string[1] { "token" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var externalResourceService = new ExternalResourceService();
            var responseData = externalResourceService.GetListByRequest(requestData);
            if (responseData == null)
            {
                response.Message = "查詢失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            response.Message = "查詢成功";
            response.Success = true;
            response.Data = responseData.ToArray();
            return Ok(response);
        }

        /// <summary>
        /// 處理同步API資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost, TokenValidation]
        public IHttpActionResult Post(ExternalResourcePostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<ExternalResourcePostRequest>();
            var response = new BaseResponse<List<ExternalResource>>();
            response.Success = false;
            response.Data = new List<ExternalResource>();
            var checkColumnKeys = new string[2] { "token", "orgcode" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var externalResourceService = new ExternalResourceService();
            var responseData = externalResourceService.DataProxy(requestData);
            if (responseData == null)
            {
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "資料處理錯誤";
                return Ok(response);
            }
            response.Success = true;
            response.Message = "資料處理成功";
            response.Data = responseData;
            return Ok(response);
        }
    }
}

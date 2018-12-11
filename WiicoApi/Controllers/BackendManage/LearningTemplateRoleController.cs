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
    /// 課程角色範本控制器
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class LearningTemplateRoleController : ApiController
    {
        /// <summary>
        /// 取得課程角色範本列表
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<BackendBaseRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<BackendBaseRequest>();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<LearningTemplateRoles>>();
            var checkColumnKeys = new string[2] { "token", "orgcode" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }

            var learningTemplateRoleService = new LearningTemplateRoleService();
            var responseData = learningTemplateRoleService.GetListByRequest(requestData);
            if (responseData == null)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "取得失敗";
                return Ok(response);
            }
            response.Success = true;
            response.Data = responseData;
            response.Message = "取得成功";
            return Ok(response);
        }
        /// <summary>
        /// 資料處理
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost, TokenValidation]
        public IHttpActionResult Post(LearningTemplateRolePostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<LearningTemplateRolePostRequest>();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<LearningTemplateRoles>>();
            var checkColumnKeys = new string[2] { "token", "orgcode" };
            var checkDataEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkDataEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Success = false;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var learningTemplateRoleService = new LearningTemplateRoleService();
            var responseData = learningTemplateRoleService.DataProxy(requestData);
            if (responseData == null)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "處理資料失敗";
                return Ok(response);
            }
            response.Success = true;
            response.Data = responseData;
            response.Message = "處理資料成功";
            return Ok(response);
        }
    }
}

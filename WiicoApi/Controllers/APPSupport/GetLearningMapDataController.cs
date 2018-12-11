using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SchoolApi;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得學習地圖
    /// </summary>
    [EnableCors("*","*","*")]
    public class GetLearningMapDataController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess) {

            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Base.BackendBaseRequest>();
            var checkColumnKeys = new string[2] { "icantoken", "account"};
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<Infrastructure.ViewModel.School.GetLearningMapDataResponse>>();
            response.Success = false;
            response.Data = new List<Infrastructure.ViewModel.School.GetLearningMapDataResponse>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (tokenInfo == null)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                response.Message = "已登出";
                return Ok(response);
            }
            var learningMapService = new SceLearningMapService();
            var responseData = learningMapService.GetData(requestData.ICanToken);
            if (responseData != null)
            {
                response.Success = true;
                response.Message = "查詢成功";
                response.Data.Add(responseData);
            }
            else
            {
                response.Success = true;
                response.Message = "老師無學習地圖資料";
            }
            return Ok(response);
        }
    }
}

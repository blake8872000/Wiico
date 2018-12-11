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
    [EnableCors("*","*","*")]
    public class GetPdsFlowsController : ApiController
    {
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.School.GetPdsFlowsRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.School.GetPdsFlowsRequest>();
            var checkColumnKeys = new string[3] { "icantoken", "coll_semegrade", "id_coll" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            //api 140.137.200.178/API/{1}coll_semegrade[學年]/{2}ID_coll分類代碼/{3}account
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<Infrastructure.ViewModel.School.PdsFlowModel>>();
            response.Success = false;
            response.Data = new List<Infrastructure.ViewModel.School.PdsFlowModel>();
            if (checkEmpty == false) {
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
            }
            var pdsService = new PDSService();
            var responseData = pdsService.GetData(requestData.Coll_SemeGrade,requestData.ID_coll,requestData.ICanToken);
            if (responseData == null)
            {
                response.Message = "查無資料";
            }
            else {
                response.Message = "查詢成功";
                response.Success = true;
                response.Data.Add(responseData);
            }
            return Ok(response);
        }
    }
}

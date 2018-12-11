using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 傳統課表
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class WeekTableManageController : ApiController
    {  /// <summary>
       /// 取得傳統課表資訊
       /// </summary>
       /// <param name="strAccess"></param>
       /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Base.BackendBaseRequest>();
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);

            var checkColumnKey = new string[1] { "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<WeekTableViewModel>();
            response.Success = false;
            response.Data = new WeekTableViewModel();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token).Result;
            if (checkToken == null)
            {
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }
            var weektableService = new WeekTableService();
            var responseData = (requestData.CircleKey == null || requestData.CircleKey == string.Empty) ?
                weektableService.GetByToken(requestData.Token) :
                weektableService.GetByCirclekey(requestData.CircleKey.ToLower());

            if (responseData == null)
            {
                response.Message = "目前無課表資訊";
                response.State = Infrastructure.ViewModel.Base.LogState.DataNotModified;
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = responseData;
            return Ok(response);
        }

        /// <summary>
        /// 處理weekTable資料
        /// </summary>
        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(WeekTablePostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<WeekTablePostRequest>();
            var checkColumnKeys = new string[3] { "token", "circlekey", "weektabledata" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = false;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var weekTableService = new WeekTableService();
            var responseData = weekTableService.WeekTableDataProxy(requestData);
            if (responseData)
            {
                response.Success = true;
                response.Message = "處理成功";
                return Ok(response);
            }
            response.Message = "處理失敗";
            response.State = Infrastructure.ViewModel.Base.LogState.Error;

            return Ok(response);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 取得天氣
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetCurrentWeatherController : ApiController
    {
        /// <summary>
        /// 取得天氣資訊
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess)
        {

            var requestData = new Infrastructure.ViewModel.Base.BackendBaseRequest();
            try
            {
                requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);
                if (requestData.Account == null  || requestData.ICanToken == null)
                    return Content(HttpStatusCode.BadRequest, "參數錯誤");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "參數錯誤");
            }
            var weatherService = new WeatherService();
            var result = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.ViewModel.Weather.WeatherDataModel>();

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;

            if (checkToken == null)
            {
                result.Success = false;
                result.Message = "已登出";
                result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(result);
            }

            var data = weatherService.GetData(requestData).Result;
            if (data == null)
            {
                result.Success = false;
                result.Message = "查無資料";
                return Ok(result);
            }

            result.Success = true;
            result.Message = "查詢成功";
            result.Data = data;
            return Ok(result);
        }
    }
}

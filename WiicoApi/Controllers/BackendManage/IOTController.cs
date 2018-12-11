using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WiicoApi.Infrastructure.ViewModel.MQTT;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.MQTT;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 教室設備資訊查詢
    /// </summary>
    public class IOTController : ApiController
    {

        /// <summary>
        /// 取得
        /// </summary>
        /// <param name="iCanToken"></param>
        /// <param name="account"></param>
        /// <param name="code"></param>
        /// <param name="classId"></param>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get([FromUri]string strAccess)
        {

            var requestData = JsonConvert.DeserializeObject<IOTGetRequest>(strAccess);
            if (requestData.ICanToken == null ||
                requestData.Account == null ||
                requestData.ClassId == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");

            var service = new MQTTDeviceService();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<RoomDeviceViewModel>();

            var responseData = service.GetRoomDevice(requestData.ClassId.ToLower()).Result;
            if (responseData == null)
            {
                response.Success = false;
                response.Message = "查詢失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = responseData;
            return Ok(response);
        }

        /// <summary>
        /// 變更sensor狀態
        /// </summary>
        /// <param name="iCanToken"></param>
        /// <param name="account"></param>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <param name="deviceId"></param>
        /// <param name="value"></param>
        /// <param name="requestData" ></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody] IOTPostRequest requestData)
        {
            if (requestData.ICanToken == null ||
                requestData.Account == null ||
                requestData.Id == null ||
                requestData.Value == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");
            var service = new MQTTDeviceService();
            var tokenService = new TokenService();
            var chekcToken = tokenService.GetTokenInfo(requestData.ICanToken);
            if (chekcToken == null)
                return Content(HttpStatusCode.Forbidden, "已登出");

            var responseData = service.UpdateIOTData(requestData.DeviceId, requestData.Id, requestData.Value).Result;
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = responseData;
            if (responseData)
            {
                response.Message = "修改成功";
                return Ok(response);
            }
            else
            {
                response.Message = "修改失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
        }
    }
}

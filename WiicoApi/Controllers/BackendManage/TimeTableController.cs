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
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 課程大綱
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class TimeTableController : ApiController
    {
        /// <summary>
        /// 取得展開上課時間
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestService = new Service.Utility.RequestDataHelper<WeekTablePostRequest>();
            var requestData = JsonConvert.DeserializeObject<WeekTablePostRequest>(strAccess);
            var checkColumnKey = new string[2] { "token", "circlekey" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<TimeTable>>();
            response.Success = false;
            response.Data = new List<TimeTable>();
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
            var responseData = new List<TimeTable>();
            if (requestData.WeekTableData != null && requestData.WeekTableData.WeekTable.Count() > 0)
            {
                var dateTimeTools = new Service.Utility.DateTimeTools();
                responseData = dateTimeTools.GetTimeTableByWeekTable(requestData);
            }
            else
            {
                var timetableService = new TimeTableService();
                var searchData = timetableService.GetList(requestData.CircleKey.ToLower());
                if (searchData != null)
                    responseData = searchData.ToList();
            }

            if (responseData == null)
            {
                response.Message = "查無資料";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = responseData;
            return Ok(response);
        }
        /// <summary>
        /// 上課時間表資料處理
        /// </summary>
        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(TimeTablePostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<TimeTablePostRequest>();
            var checkColumnKeys = new string[3] { "token", "circlekey", "timetable" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = false;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var timeTableService = new TimeTableService();
            var responseData = timeTableService.TimeTableDataProxy(requestData);
            if (responseData)
            {
                response.Success = true;
                response.Message = "處理資料成功";
                return Ok(response);
            }
            response.Message = "處理資料失敗";
            response.State = Infrastructure.ViewModel.Base.LogState.Error;
            return Ok(response);
        }
    }
}

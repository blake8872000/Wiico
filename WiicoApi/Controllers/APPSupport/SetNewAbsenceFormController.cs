using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Controllers.Common;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.Leave;
using WiicoApi.Service.SignalRService;
using WiicoApi.SignalR;
using WiicoApi.SignalRHub;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 建立假單
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SetNewAbsenceFormController : MultipartFormDataFilesController<SetNewAbsenceFormRequest>
    {
        private IHubContext objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoHub>();
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();

        /// <summary>
        /// 建立請假單 - Get方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task< IHttpActionResult> Get() {
         
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            multipartFormDataModel = new SetNewAbsenceFormRequest();
            await SetFileData();
            if (multipartFormDataStreamProvider.FormData == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            //設定參數
            var token = multipartFormDataStreamProvider.FormData.Get("icantoken") != null ? multipartFormDataStreamProvider.FormData.GetValues("icantoken")[0] : null;
            var circleKey = multipartFormDataStreamProvider.FormData.Get("classid") != null ? multipartFormDataStreamProvider.FormData.GetValues("classid")[0] : null;
            var title = multipartFormDataStreamProvider.FormData.Get("title") != null ? multipartFormDataStreamProvider.FormData.GetValues("title")[0] : null;
            var content = multipartFormDataStreamProvider.FormData.Get("content") != null ? multipartFormDataStreamProvider.FormData.GetValues("content")[0] : null;
            var leavedate = multipartFormDataStreamProvider.FormData.Get("leavedate") != null ? multipartFormDataStreamProvider.FormData.GetValues("leavedate")[0] : null;
            var leavecategoryid = multipartFormDataStreamProvider.FormData.Get("leavecategoryid") != null ? multipartFormDataStreamProvider.FormData.GetValues("leavecategoryid")[0] : null;

            if (token == null || circleKey == null || title == null || content == null || leavedate == null || leavecategoryid == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            multipartFormDataModel.Token = Guid.Parse(token);
            multipartFormDataModel.CircleKey = circleKey.ToLower();
            multipartFormDataModel.Title = HttpUtility.UrlDecode(title);
            multipartFormDataModel.Content = HttpUtility.UrlDecode(content);
            multipartFormDataModel.LeaveDate = Convert.ToDateTime(leavedate);
            multipartFormDataModel.LeaveCategoryId = Convert.ToInt32(leavecategoryid);

            var service = new LeaveService();

            var data = service.SetNewAbsenceForm(multipartFormDataModel, multipartFormDataFiles,fileStreams);
            objHub.Clients.Group(multipartFormDataModel.CircleKey.ToLower()).addLeave(data);


            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(multipartFormDataModel.Token.ToString()).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }

            response.Success = data != null ? true : false;
            response.Message = data != null ? "建立成功" : "建立失敗";

            //單元測試結果
            if (HttpContext.Current == null)
                return Ok(response);

            var pushService = new PushService();
            // 檢查通過，save data
            var message = pushService.PushOnCreatedLeave(multipartFormDataModel.CircleKey, data.EventId, checkToken.MemberId, multipartFormDataModel.LeaveDate.ToLocalTime()).Result;
            var activityService = new ActivityService();

            var signalrService = new SignalrService();

            //發通知給老師們
            var connectIdAndNoticeData = signalrService.GetConnectIdAndData(multipartFormDataModel.CircleKey, checkToken.MemberId, SignalrConnectIdType.Teachers,SignalrDataType.Notice);
            var learningCircleService = new LearningCircleService();
            var teachers = learningCircleService.GetCircleTeacherIdListBySql(multipartFormDataModel.CircleKey.ToLower(),checkToken.MemberId);
            // 將通知寫入資料庫
            activityService.AddMutipleNotice(multipartFormDataModel.CircleKey, teachers, data.EventId, message);
            SignalrClientHelper.SendNotice(connectIdAndNoticeData);

            foreach (var item in data.OuterKeySignInLog)
            {
                //告訴同班的有活動的開始時間被修改
                SignalrClientHelper.SignIn_StatusChanged(multipartFormDataModel.CircleKey, item.Key, item.Value);
            }
            //    return result;
            return Ok(response);
        }
        /// <summary>
        /// 建立一筆假單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> Post() {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            multipartFormDataModel = new SetNewAbsenceFormRequest();
            await SetFileData();
            if (multipartFormDataStreamProvider.FormData == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            //設定參數
            var token = multipartFormDataStreamProvider.FormData.Get("icantoken") != null ? multipartFormDataStreamProvider.FormData.GetValues("icantoken")[0] : null;
            var circleKey = multipartFormDataStreamProvider.FormData.Get("classid") != null ? multipartFormDataStreamProvider.FormData.GetValues("classid")[0] : null;
            var title = multipartFormDataStreamProvider.FormData.Get("title") != null ? multipartFormDataStreamProvider.FormData.GetValues("title")[0] : null;
            var content = multipartFormDataStreamProvider.FormData.Get("content") != null ? multipartFormDataStreamProvider.FormData.GetValues("content")[0] : null;
            var leavedate = multipartFormDataStreamProvider.FormData.Get("leavedate") != null ? multipartFormDataStreamProvider.FormData.GetValues("leavedate")[0] : null;
            var leavecategoryid = multipartFormDataStreamProvider.FormData.Get("leavecategoryid") != null ? multipartFormDataStreamProvider.FormData.GetValues("leavecategoryid")[0] : null;

            if (token == null || circleKey == null || title == null || content == null || leavedate == null || leavecategoryid == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            multipartFormDataModel.Token = Guid.Parse(token);
            multipartFormDataModel.CircleKey = circleKey.ToLower();
            multipartFormDataModel.Title = HttpUtility.UrlDecode(title);
            multipartFormDataModel.Content = HttpUtility.UrlDecode(content);
            multipartFormDataModel.LeaveDate = Convert.ToDateTime(leavedate);
            multipartFormDataModel.LeaveCategoryId = Convert.ToInt32(leavecategoryid);

            var service = new LeaveService();
            var data = service.SetNewAbsenceForm(multipartFormDataModel,multipartFormDataFiles,fileStreams);
            if (data == null)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                response.Message = "建立失敗";
                return Ok(response);
            }
            
            objHub.Clients.Group(multipartFormDataModel.CircleKey.ToLower()).addLeave(data);
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(multipartFormDataModel.Token.ToString()).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Content(HttpStatusCode.BadRequest, response);
            }

            response.Success = data!=null ? true : false ;
            response.Message = data!=null ? "建立成功" : "建立失敗";

            //單元測試結果
            if (HttpContext.Current == null)
                return Ok(response);

            var pushService = new PushService();
            // 檢查通過，save data
            var message = pushService.PushOnCreatedLeave(multipartFormDataModel.CircleKey, data.EventId, checkToken.MemberId, multipartFormDataModel.LeaveDate.ToLocalTime()).Result;

            var learningCircleService = new LearningCircleService();
            var teachers = learningCircleService.GetCircleTeacherIdListBySql(multipartFormDataModel.CircleKey.ToLower(), checkToken.MemberId);
            var activityService = new ActivityService();
            // 將通知寫入資料庫
            activityService.AddMutipleNotice(multipartFormDataModel.CircleKey, teachers, data.EventId, message);
            var signalrService = new SignalrService();
            //發通知給老師們
            var connectIdAndNoticeData = signalrService.GetConnectIdAndData(multipartFormDataModel.CircleKey, checkToken.MemberId, SignalrConnectIdType.Teachers, SignalrDataType.Notice);
            SignalrClientHelper.SendNotice(connectIdAndNoticeData);
            if (data.OuterKeySignInLog != null) {
                foreach (var item in data.OuterKeySignInLog)
                {
                    //告訴同班的有活動的開始時間被修改
                    SignalrClientHelper.SignIn_StatusChanged(multipartFormDataModel.CircleKey, item.Key, item.Value);
                }
            }
          
            //    return result;
            return Ok(response);
        }
    }
}

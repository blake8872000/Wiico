using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.Leave;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.SignIn;
using WiicoApi.Service.Utility;
using WiicoApi.SignalR;
using WiicoApi.SignalRHub;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 老師變更假單狀態
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SetAbsenceFormStatusController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusRequest>(strAccess);
            if (requestData.Account == null ||
                requestData.ClassID == null ||
                requestData.ICanToken == null ||
                requestData.OuterKey == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數");

            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse>();

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }

            var learningService = new LearningCircleService();
            var learningId = learningService.GetDetailByOuterKey(requestData.ClassID).Id;
            var authService = new AuthService();
   
            // 是否有審核請假單的權限
            var reviewLeave = authService.CheckFunctionAuth(learningId, ParaCondition.LeaveFunction.Review, checkToken.MemberId);
            var activityService = new ActivityService();
            var service = new LeaveService();
            var pushService = new PushService();
            var signalrService = new SignalrService();
            var signInLogService = new SignInLogService();
            var reViewData = new List<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse>();
            //設定審核狀態
            var status = string.Empty;
            switch (requestData.status)
            {
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Invalid:
                    status = "00";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Pass:
                    status = "10";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Wait:
                    status = "20";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Recall:
                    status = "30";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Reject:
                    status = "40";
                    break;
                default:
                    break;
            }
            foreach (var leave in requestData.OuterKey)
            {
                var eventId = Service.Utility.OuterKeyHelper.CheckOuterKey(leave);
                if (eventId.HasValue == false)
                    continue;
                //修改狀態
                var data = service.Update(eventId.Value, null, null, requestData.reason, status);
                //修改點名狀態
                //老師審核
                if (reviewLeave)
                {
                    var compareDay = activityService.GetActivitys(requestData.ClassID, data.LeaveDate);
                    if (requestData.status==  enumAbsenceFormStatus.Pass)
                    {
                        response.Message = "審核成功";
               
                        foreach (var _signInInfo in compareDay)
                        {
                            var resReviewData = new Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse();
                            var signInOuterKey =  OuterKeyHelper.GuidToPageToken(_signInInfo.OuterKey);
                            // 更新點名紀錄為請假   { 0, "未開放您參加此活動"},  { 1, "出席"}, { 2, "缺席"}, { "3", "遲到"}, { "4", "早退"},{"5", "請假"}
                            var signInlogInfo = signInLogService.GetSignInLog(checkToken.MemberId, _signInInfo.OuterKey, data.StudId);
                            if (signInlogInfo.Status == "2")
                            {
                                var updateSignInLog = signInLogService.UpdateLog(checkToken.MemberId, _signInInfo.OuterKey, data.StudId, "5");

                                //var mylog = _logService.GetLogs(eventId);
                                updateSignInLog.LeaveStatus = requestData.status.ToString();
                                SignalrClientHelper.SignIn_StatusChanged(requestData.ClassID, signInOuterKey, updateSignInLog);
                                if (_signInInfo.StartDate.Value != null)
                                {
                                    resReviewData.SignInOuterKey = signInOuterKey;
                                    resReviewData.SignInDateTime = _signInInfo.StartDate.Value;
                                    reViewData.Add(resReviewData);
                                }
                            }
                            else
                            {
                                var mylog = signInLogService.GetSignInLog(Convert.ToInt32(_signInInfo.CreateUser.ToString()), _signInInfo.OuterKey, data.StudId);
                                if (_signInInfo.StartDate.Value != null)
                                {
                                    resReviewData.SignInOuterKey = signInOuterKey;
                                    resReviewData.SignInDateTime = _signInInfo.StartDate.Value;
                                    reViewData.Add(resReviewData);
                                }
                                SignalrClientHelper.SignIn_StatusChanged(requestData.ClassID, signInOuterKey, mylog);
                            }
                            //_array.SetValue(signInOuterKey, _index);
                            // _index++;
                        }
                        response.Success = true;
                                 //推播
                         pushService.PushOnUpdatedLeave(requestData.ClassID, eventId.Value, data.StudId, data.LeaveDate.ToLocalTime(), "已通過");
                        var noticeMsg = string.Format("您在({0:yyyy/MM/dd})的請假單「{1}」", data.LeaveDate.ToLocalTime(), "已通過", requestData.ClassID, DateTime.Now);
                              // 將通知寫入資料庫
                        activityService.AddNotice(requestData.ClassID.ToLower(), data.StudId, data.EventId, noticeMsg);
                        //發通知給學生
                        var connectIdAndNoticeData = signalrService.GetConnectIdAndData(requestData.ClassID, data.StudId, SignalrConnectIdType.One, SignalrDataType.Notice);
                        SignalrClientHelper.SendNotice(connectIdAndNoticeData);

                    } //駁回
                    else if (requestData.status== enumAbsenceFormStatus.Reject)
                    {
                        response.Message = string.Format("駁回，原因:{0}", requestData.reason);
                   
                        foreach (var _signInInfo in compareDay)
                        {
                            var resReviewData = new Infrastructure.ViewModel.ReviewDataModel();
                            // var signInLog = _logService.GetLogs(_signInInfo.EventId);
                            var mylog = signInLogService.GetSignInLog(Convert.ToInt32(_signInInfo.CreateUser.ToString()), _signInInfo.OuterKey, data.StudId);
                            mylog.LeaveStatus = requestData.status.ToString();
                            var signInOuterKey = Service.Utility.OuterKeyHelper.GuidToPageToken(_signInInfo.OuterKey);
                            //var mylog = _logService.GetLogs(eventId);
                            SignalrClientHelper.SignIn_StatusChanged(requestData.ClassID, signInOuterKey, mylog);
                            //if (_signInInfo.Updated != null)
                            //{
                            //    resReviewData.SignInOuterKey = signInOuterKey;
                            //    resReviewData.SignInDateTime = _signInInfo.Updated.Local.Value;
                            //    reViewData.Add(resReviewData);
                            //}
                            // _array.SetValue(signInOuterKey, _index);
                            // _index++;
                        }
                        response.Success = true;
                        //推播
                        pushService.PushOnUpdatedLeave(requestData.ClassID, eventId.Value, data.StudId, data.LeaveDate.ToLocalTime(), "未通過");
                        var noticeMsg = string.Format("您在({0:yyyy/MM/dd})的請假單「{1}」", data.LeaveDate.ToLocalTime(), "未通過", requestData.ClassID, DateTime.Now);

                        // 將通知寫入資料庫
                        activityService.AddNotice(requestData.ClassID, data.StudId, eventId.Value, noticeMsg);

                        //發通知給學生
                        var connectIdAndNoticeData = signalrService.GetConnectIdAndData(requestData.ClassID, data.StudId, SignalrConnectIdType.One, SignalrDataType.Notice);
                        SignalrClientHelper.SendNotice(connectIdAndNoticeData);

                    }
                }//學生抽回
                else
                {
                    if (requestData.status== enumAbsenceFormStatus.Recall)
                    {
                        response.Message = "已抽回";

                        if (data != null)
                            response.Success = true;
                        else
                        {
                            response.State = Infrastructure.ViewModel.Base.LogState.Error;
                            response.Success = false;
                            response.Message = "查無此請假單";
                        }
                    }
                    else
                    {
                        response.State = Infrastructure.ViewModel.Base.LogState.Error;
                        response.Message = "無審核權限";
                        response.Success = false;
                    }
                }
                //發送推播
            }

            var _finalReviewData = new List<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse>();
            foreach (var _item in reViewData)
            {
                var che = _finalReviewData.Where(t => t.SignInOuterKey == _item.SignInOuterKey);
                if (!che.Any())
                {
                    _finalReviewData.Add(_item);
                }
            }
            response.Data = _finalReviewData.ToArray();
            return Ok(response);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody]Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse>();

            if (requestData.Account == null ||
                requestData.ClassID == null ||
                requestData.ICanToken == null ||
                requestData.OuterKey == null) {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Data = new Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse[0];
                response.Message = "遺漏參數";
                return Content(HttpStatusCode.BadRequest, response);
            }


            var learningService = new LearningCircleService();
            var learningId = learningService.GetDetailByOuterKey(requestData.ClassID).Id;
            var authService = new AuthService();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null) {
                response.Success = false;
                response.Data = new Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse[0];
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }
     
            // 是否有審核請假單的權限
            var reviewLeave = authService.CheckFunctionAuth(learningId,ParaCondition.LeaveFunction.Review, checkToken.MemberId);
            var activityService = new ActivityService();
            var service = new LeaveService();
            var pushService = new PushService();
            var signalrService = new SignalrService();
            var signInLogService = new SignInLogService();
            var reViewData = new List<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse>();
            //設定審核狀態
            var status = string.Empty;
            switch (requestData.status)
            {
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Invalid:
                    status = "00";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Pass:
                    status = "10";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Wait:
                    status = "20";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Recall:
                    status = "30";
                    break;
                case Infrastructure.ValueObject.enumAbsenceFormStatus.Reject:
                    status = "40";
                    break;
                default:
                    break;
            }
            foreach (var leave in requestData.OuterKey)
            {
                var eventId = Service.Utility.OuterKeyHelper.CheckOuterKey(leave);
                if (eventId.HasValue == false)
                    continue;
                //修改狀態
                var data = service.Update(eventId.Value, null, null, requestData.reason, status);
                //修改點名狀態
                //老師審核
                if (reviewLeave)
                {
                    var compareDay = activityService.GetActivitys(requestData.ClassID, data.LeaveDate);
                    if (requestData.status == enumAbsenceFormStatus.Pass)
                    {
                        response.Message = "審核成功";

                        foreach (var _signInInfo in compareDay)
                        {
                            var resReviewData = new Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse();
                            var signInOuterKey = Service.Utility.OuterKeyHelper.GuidToPageToken(_signInInfo.OuterKey);
                            // 更新點名紀錄為請假   { 0, "未開放您參加此活動"},  { 1, "出席"}, { 2, "缺席"}, { "3", "遲到"}, { "4", "早退"},{"5", "請假"}
                            var signInlogInfo = signInLogService.GetSignInLog(checkToken.MemberId, _signInInfo.OuterKey, data.StudId);
                            if (signInlogInfo.Status == "2")
                            {
                                var updateSignInLog = signInLogService.UpdateLog(checkToken.MemberId, _signInInfo.OuterKey, data.StudId, "5");

                                //var mylog = _logService.GetLogs(eventId);
                                updateSignInLog.LeaveStatus = requestData.status.ToString();
                                SignalrClientHelper.SignIn_StatusChanged(requestData.ClassID, signInOuterKey, updateSignInLog);
                                if (_signInInfo.StartDate.Value != null)
                                {
                                    resReviewData.SignInOuterKey = signInOuterKey;
                                    resReviewData.SignInDateTime = _signInInfo.StartDate.Value;
                                    reViewData.Add(resReviewData);
                                }

                            }
                            else
                            {
                                var mylog = signInLogService.GetSignInLog(Convert.ToInt32(_signInInfo.CreateUser.ToString()), _signInInfo.OuterKey, data.StudId);
                                if (_signInInfo.StartDate.Value != null)
                                {
                                    resReviewData.SignInOuterKey = signInOuterKey;
                                    resReviewData.SignInDateTime = _signInInfo.StartDate.Value;
                                    reViewData.Add(resReviewData);
                                }
                                SignalrClientHelper.SignIn_StatusChanged(requestData.ClassID, signInOuterKey, mylog);
                            }
                            //_array.SetValue(signInOuterKey, _index);
                            // _index++;
                        }
                        //推播
                       pushService.PushOnUpdatedLeave(requestData.ClassID, eventId.Value, data.StudId, data.LeaveDate.ToLocalTime(), "已通過");
                        var noticeMsg = string.Format("您在({0:yyyy/MM/dd})的請假單「{1}」", data.LeaveDate.ToLocalTime(), "已通過", requestData.ClassID, DateTime.Now);

                        // 將通知寫入資料庫
                        activityService.AddNotice(requestData.ClassID.ToLower(), data.StudId, data.EventId, noticeMsg);
                        //發通知給學生
                        var connectIdAndNoticeData = signalrService.GetConnectIdAndData(requestData.ClassID, data.StudId, SignalrConnectIdType.One, SignalrDataType.Notice);

                        SignalrClientHelper.SendNotice(connectIdAndNoticeData);
                        response.Success = true;
                    } //駁回
                    else if (requestData.status == enumAbsenceFormStatus.Reject)
                    {
                        response.Message = string.Format("駁回，原因:{0}", requestData.reason);
                  
                        foreach (var _signInInfo in compareDay)
                        {
                            var resReviewData = new Infrastructure.ViewModel.ReviewDataModel();
                            // var signInLog = _logService.GetLogs(_signInInfo.EventId);
                            var mylog = signInLogService.GetSignInLog(Convert.ToInt32(_signInInfo.CreateUser.ToString()), _signInInfo.OuterKey, data.StudId);
                            mylog.LeaveStatus = requestData.status.ToString();
                            var signInOuterKey = Service.Utility.OuterKeyHelper.GuidToPageToken(_signInInfo.OuterKey);
                            //var mylog = _logService.GetLogs(eventId);
                            SignalrClientHelper.SignIn_StatusChanged(requestData.ClassID, signInOuterKey, mylog);
                            //if (_signInInfo.Updated != null)
                            //{
                            //    resReviewData.SignInOuterKey = signInOuterKey;
                            //    resReviewData.SignInDateTime = _signInInfo.Updated.Local.Value;
                            //    reViewData.Add(resReviewData);
                            //}
                            // _array.SetValue(signInOuterKey, _index);
                            // _index++;
                        }
                        //推播
                     pushService.PushOnUpdatedLeave(requestData.ClassID, eventId.Value, data.StudId, data.LeaveDate.ToLocalTime(), "未通過");
                        var noticeMsg = string.Format("您在({0:yyyy/MM/dd})的請假單「{1}」", data.LeaveDate.ToLocalTime(), "未通過", requestData.ClassID, DateTime.Now);

                        // 將通知寫入資料庫
                        activityService.AddNotice(requestData.ClassID, data.StudId, eventId.Value, noticeMsg);
                        //發通知給學生
                        var connectIdAndNoticeData = signalrService.GetConnectIdAndData(requestData.ClassID, data.StudId, SignalrConnectIdType.One, SignalrDataType.Notice);
                        SignalrClientHelper.SendNotice(connectIdAndNoticeData);
                        response.Success = true;
                    }
                }//學生抽回
                else
                {
                    if (requestData.status == enumAbsenceFormStatus.Recall)
                    {
                        response.Message = "已抽回";

                        if (data != null)
                            response.Success = true;
                        else
                        {
                            response.Success = false;
                            response.Message = "查無此請假單";
                        }
                    }
                    else
                    {
                        response.Message = "無審核權限";
                        response.Success = false;
                    }
                }
                //發送推播
            }

            var _finalReviewData = new List<Infrastructure.ViewModel.ActivityFunction.Leave.SetAbsenceFormStatusResponse>();
            foreach (var _item in reViewData)
            {
                var che = _finalReviewData.Where(t => t.SignInOuterKey == _item.SignInOuterKey);
                if (!che.Any())
                {
                    _finalReviewData.Add(_item);
                }
            }
            response.Data = _finalReviewData.ToArray();
            return Ok(response);
        }
    }
}

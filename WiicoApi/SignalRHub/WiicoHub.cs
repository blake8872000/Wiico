using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.ActivityModule;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.Leave;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;
using WiicoApi.Service.SignalRService.SignIn;
using WiicoApi.Service.Utility;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.SignalRHub
{
    /// <summary>
    /// SignalRHub
    /// </summary>
    public partial class WiicoHub : WiicoHubBase
    {
        private MemberService memberService = new MemberService();
        private NoticeService noticeService = new NoticeService();
        private CacheService cacheService = new CacheService();
        private ActivityService activityService = new ActivityService();
        private LearningCircleService learningCircleService = new LearningCircleService();
        private ErrorService errorService = new ErrorService();
        const int actMaxResult = 20;
        const int noticeMaxResult = 20;
        /// <summary>
        /// 驗證token取出成員資訊
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Infrastructure.Entity.Member CheckTokenToMemberInfo(Guid token)
        {
            var checkToken = CheckToken(token.ToString());
            if (checkToken == null)
                return null;
            return memberService.UserIdToAccount(checkToken.MemberId);
        }

        /// <summary>
        /// 取得某個人學習圈列表
        /// </summary>
        /// <param name="token"></param>
        public BaseResponse<List<LearningCircle>> getGroup(Guid token)
        {
            var response = new BaseResponse<List<LearningCircle>>();
            var checkMember = CheckToken(token.ToString());
            response.Success = false;
            response.Data = new List<LearningCircle>();

            if (checkMember == null)
                return response;
            try
            {
                if (checkMember != null)
                {
                    var list = learningCircleService.GetListByMemberId(checkMember.MemberId);
                    Clients.Caller.showCircleList(list);
                    response.Success = true;
                    response.Data = list;
                }
                return response;
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("getGroup", "取得學習圈列表失敗: token:[" + token + "]" + ex.Message);
                return response;
            }
        }



        /// <summary>
        /// 日期查詢活動(取得水道清單列表)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="maxResult"></param>
        /// <param name="outerKeyString"></param>
        /// <param name="queryDate"></param>
        public void LoadQueryDateActivities(Guid token, string circleKey, DateTime? queryDate = null, int maxResult = actMaxResult, string outerKeyString = null)
        {
            var checkMember = CheckToken(token.ToString());
            if (checkMember == null)
            {
                Clients.Caller.onError("LoadInitialActivities", "身分驗證失敗，請重新登入!token:[" + token + "]");
                return;
            }
            try
            {

                var eventId = OuterKeyHelper.CheckOuterKey(outerKeyString);
                if (eventId == null)
                {
                    Clients.Caller.onError("LoadInitialActivities", "取得錯誤活動代碼，outerKey:[" + outerKeyString + "]");
                    return;
                }
                var model = activityService.GetQueryDateList(circleKey.ToLower(), checkMember.MemberId, maxResult, eventId.Value, queryDate);

                if (model.Success)
                    Clients.Caller.showInitActivities(model);
                else
                    Clients.Caller.onError("LoadInitialActivities", model.Message);
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("LoadInitialActivities", "取得活動列表發生意外: " + msg);
            }
        }
        public ResultBaseModel<JObject> LoadActivityDetail(Guid token, string moduleKey, string outerKey, int? maxResult = 10)
        {
            var responseCommonData = new ResultBaseModel<JObject>();
            responseCommonData.Success = false;
            var checkMember = CheckToken(token.ToString());
            if (checkMember == null)
            {
                responseCommonData.Message = "身分驗證失敗，請重新登入";
                var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseCommonData, null);
                return responseCommonData;
            }

            var version = 0;

            if (Context.QueryString != null && !string.IsNullOrEmpty(Context.QueryString["version"]))
                version = Convert.ToInt32(Context.QueryString["version"]);

            if (checkMember.MemberId > 0 && outerKey != string.Empty && outerKey != null)
            {
                var eventId = OuterKeyHelper.CheckOuterKey(outerKey);
                if (eventId.HasValue == false)
                {
                    responseCommonData.Success = false;
                    responseCommonData.Message = "outerKey格式錯誤";
                    var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseCommonData, new Controllers.APPSupport.EmptyController());
                    return responseCommonData;
                }
                var activityInfo = activityService.GetByEventId(eventId.Value);
                if (activityInfo == null)
                {
                    responseCommonData.Success = false;
                    responseCommonData.Message = "查無該活動";
                    var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseCommonData, new Controllers.APPSupport.EmptyController());
                    return responseCommonData;
                }
                var learningCircleService = new LearningCircleService();
                var learningCircleInfo = learningCircleService.GetDetailByOuterKey(activityInfo.ToRoomId.ToLower());
                if (learningCircleInfo == null)
                {
                    responseCommonData.Success = false;
                    responseCommonData.Message = "查無學習圈資訊";
                    var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseCommonData, new Controllers.APPSupport.EmptyController());
                    return responseCommonData;
                }

                var JObjectListData = new List<JObject>();
                switch (moduleKey.ToLower())
                {
                    case "signin":
                        var responseSigninContent = new ResultBaseModel<JObject>();
                        try
                        {
                            var signInService = new SignInService();
                            var responseData = signInService.GetSignInEvent(eventId.Value, checkMember.MemberId);
                            responseSigninContent.Success = true;
                            var responseJObjectData = JObject.FromObject(responseData);
                            JObjectListData.Add(responseJObjectData);
                            responseSigninContent.Data = JObjectListData.ToArray();
                            Clients.Caller.renderDetails(ModuleType.SignIn, responseData);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseSigninContent, new Controllers.APPSupport.EmptyController());
                            return responseSigninContent;
                        }
                        catch (Exception ex)
                        {
                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                            responseSigninContent.Message = "載入點名細節發生意外";
                            responseSigninContent.Success = false;
                            errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得點名活動發生意外", ex.Message));
                            Clients.Caller.onError("SignIn_LoadDetails", "載入點名細節發生意外: " + msg);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseSigninContent, new Controllers.APPSupport.EmptyController());
                            return responseSigninContent;
                        }
                    case "message":
                        var responseMsgContent = new ResultBaseModel<JObject>();
                        try
                        {
                            var messageService = new MessageService();
                            var responseData = messageService.Get(eventId.Value);
                            if (responseData == null)
                            {
                                responseMsgContent.Success = false;
                                responseMsgContent.Message = "查無訊息";
                            }
                            else
                                responseMsgContent.Success = true;
                            Clients.Caller.renderDetails(ModuleType.Message, responseData);

                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseMsgContent, new Controllers.APPSupport.EmptyController());
                            return responseMsgContent;
                        }
                        catch (Exception ex)
                        {
                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                            Clients.Caller.onError("Message_LoadDetails", "載入訊息細節發生意外: " + msg);
                            responseMsgContent.Success = false;
                            responseMsgContent.Message = string.Format("{0}{1}", "Message_LoadDetails載入訊息細節發生意外: ", msg);
                            errorService.InsertError((int)SystemErrorTypeNum.SignalRError, "Message_LoadDetails載入訊息細節發生意外:" + msg);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseMsgContent, new Controllers.APPSupport.EmptyController());
                            return responseMsgContent;
                        }
                    case "upload":
                        //try
                        //{
                        //    var upload = _hwService.Rep.GetSingle(x => x.EventId == eventId);

                        //    // 是否有編輯作業活動權限，有就將所有作業紀錄取出
                        //    var hasAuth = Circle.CheckFunctionAuth(upload.LearningId, HomeWorkState.HomeWorkFunction.manageHomeWork.ToString(), memberId);
                        //    if (hasAuth)
                        //    {
                        //        //取出所有成員的作業紀錄
                        //        var obj = _hwService.GetForAll(eventId.Value);
                        //        Clients.Caller.renderDetails("upload", obj);
                        //    }
                        //    else
                        //    {
                        //        //只取出自己的作業紀錄
                        //        var obj = _hwService.GetForMem(eventId.Value, memberId);
                        //        Clients.Caller.renderDetails("upload", obj);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                        //    Clients.Caller.onError("HomeWork_LoadDetails", "建立作業活動發生意外: " + msg);
                        //}
                        break;
                    case "material":
                        var responseMaterialContent = new ResultBaseModel<JObject>();
                        try
                        {

                            var materialService = new MaterialService();
                            var responseFile = materialService.GetFile(outerKey);
                            if (responseFile != null)
                            {
                                responseMaterialContent.Success = true;
                                var responseJObjectData = JObject.FromObject(responseFile);
                                JObjectListData.Add(responseJObjectData);
                                responseMaterialContent.Data = JObjectListData.ToArray();
                            }
                            else
                            {
                                responseMaterialContent.Success = false;
                                responseMaterialContent.Data = new JObject[0];
                            }
                            Clients.Caller.renderDetails("material", responseFile);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseMaterialContent, new Controllers.APPSupport.EmptyController());
                            return responseMaterialContent;
                        }
                        catch (Exception ex)
                        {
                            responseMaterialContent.Success = false;
                            responseMaterialContent.Data = new JObject[0];
                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                            responseMaterialContent.Message = string.Format("{0}{1}", "Material_LoadDetails 取得檔案圖片活動發生意外", msg);
                            errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得圖片活動發生意外", ex.Message));
                            Clients.Caller.onError("Material_LoadDetails", "取得檔案圖片活動發生意外: " + msg);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseMaterialContent, new Controllers.APPSupport.EmptyController());
                            return responseMaterialContent;
                        }
                    case "group":
                    /*    try
                        {
                            var groupService = new GroupService();
                            var obj = groupService.GetGroupListViewModel(outerKey);
                            obj.Token = token;

                            if (obj != null)
                            {
                                response.Success = true;
                                Clients.Caller.renderDetails("group", obj);
                            }
                            else
                            {
                                response.Message = "找不到資料";
                                response.Success = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Message = "查詢分組活動發生意外";
                            response.Success = false;
                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                            Clients.Caller.onError("Group_LoadDetails", "查詢分組活動發生意外: " + msg);
                        }
                        break;*/
                    case "circlesetting":
                        break;
                    case "leave":
                        var responseLeaveContent = new ResultBaseModel<JObject>();
                        try
                        {
                            var leaveService = new LeaveService();
                            var authService = new AuthService();

                            // 是否有建立請假單權限
                            var hasAuth = authService.CheckFunctionAuth(learningCircleInfo.Id, LeaveFunction.Create, checkMember.MemberId);
                            if (hasAuth)
                            {
                                responseLeaveContent.Success = true;

                                //取出請假單資訊-學生
                                var obj = leaveService.Get(learningCircleInfo.Id, false, checkMember.MemberId, null);
                                var responseJObjectData = JObject.FromObject(obj);
                                JObjectListData.Add(responseJObjectData);
                                responseLeaveContent.Data= JObjectListData.ToArray();
                                Clients.Caller.renderDetails("leave", obj);
                            }
                            else
                            {
                                responseLeaveContent.Success = true;
                                //取出請假單資訊-老師
                                var obj = leaveService.Get(learningCircleInfo.Id, true, null, null);
                                var responseJObjectData = JObject.FromObject(obj);
                                JObjectListData.Add(responseJObjectData);
                                responseLeaveContent.Data = JObjectListData.ToArray();
                                Clients.Caller.renderDetails("leave", obj);
                            }
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseLeaveContent, new Controllers.APPSupport.EmptyController());
                            return responseLeaveContent;
                        }
                        catch (Exception ex)
                        {
                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                            responseLeaveContent.Success = false;
                            responseLeaveContent.Message = string.Format("{0}{1}", "Leave_LoadDetails 取得請假單資訊發生意外:", msg);
                            errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得假單資料發生意外", ex.Message));
                            Clients.Caller.onError("Leave_LoadDetails", "取得請假單資訊發生意外: " + msg);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseLeaveContent, new Controllers.APPSupport.EmptyController());
                            return responseLeaveContent;
                        }
                    case "discussion":
                        var responseDiscussionContent = new ResultBaseModel<JObject>();
                        try
                        {
                            var discussionService = new DiscussionService();
                            var responseData = discussionService.GetDetailByOuterKey(outerKey, maxResult);
                            responseDiscussionContent.Success = responseData != null ? true : false;
                            responseDiscussionContent.Message = responseData != null ? "查詢成功" : "查詢失敗";
                            var responseJObjectData = JObject.FromObject(responseData);
                            JObjectListData.Add(responseJObjectData);
                            var responseDiscussionData = responseData != null ? JObjectListData.ToArray() : new JObject[0];

                            responseDiscussionContent.Data = responseDiscussionData;
                            Clients.Caller.renderDetails("discussion", responseDiscussionData);

                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseDiscussionContent, new Controllers.APPSupport.EmptyController());
                            return responseDiscussionContent;
                        }
                        catch (Exception ex)
                        {
                            responseDiscussionContent.Message = "取得主題討論活動發生意外";
                            responseDiscussionContent.Success = false;
                            responseDiscussionContent.Data = new JObject[0];

                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                            errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得主題討論活動發生意外", ex.Message));
                            Clients.Caller.onError("LoadActivityDetail", "取得主題討論活動發生意外: " + msg);

                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseDiscussionContent, new Controllers.APPSupport.EmptyController());
                            return responseDiscussionContent;
                        }
                    //case "general":
                    //    try
                    //    {
                    //        var obj = _agService.Get(eventId.Value);
                    //        if (obj != null)
                    //        {
                    //            response.Success = true;
                    //            Clients.Caller.renderDetails("general", obj);
                    //        }
                    //        else
                    //        {
                    //            response.Message = "找不到資料";
                    //            response.Success = false;
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        response.Message = "取得公版活動發生意外";
                    //        response.Success = false;
                    //        var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                    //        errorService.InsertError(2, string.Format("{0}:「{1}」", "取得公版活動發生意外", ex.Message));
                    //        Clients.Caller.onError("general_LoadDetails", "取得公版活動發生意外: " + msg);
                    //    }
                    //    break;
                    case "vote":
                        var responseVoteContent = new ResultBaseModel<JObject>();
                        try
                        {
                            var voteService = new VoteService();
                            var obj = voteService.GetDetail(outerKey);
                            if (obj != null)
                            {
                                voteOuterKey = outerKey;
                                if (obj.IsStart)
                                    ConnectionSensor(obj.ToRoomId, outerKey);
                                else
                                    StopConnectionSensor();
                                responseVoteContent.Success = true;
                                Clients.Caller.renderDetails(Service.Utility.ParaCondition.ModuleType.Vote, obj);
                                var responseJObjectData = JObject.FromObject(obj);
                                JObjectListData.Add(responseJObjectData);
                                responseVoteContent.Data = JObjectListData.ToArray();
                            }
                            else
                            {
                                responseVoteContent.Success = false;
                                responseVoteContent.Message = "找不到資料";
                                responseVoteContent.Data = new JObject[0];
                            }
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseVoteContent, new Controllers.APPSupport.EmptyController());
                            return responseVoteContent;
                        }
                        catch (Exception ex)
                        {
                            responseVoteContent.Message = "取得投票活動發生意外";
                            responseVoteContent.Success = false;
                            var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                            errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得投票活動發生意外", msg));
                            //    Clients.Caller.onError("vote_LoadDetails", "取得投票活動發生意外: " + msg);
                            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseVoteContent, new Controllers.APPSupport.EmptyController());
                            return responseVoteContent;
                        }
                    default:
                        break;
                }
                //  }
                //  result.Success = isPublish;
            }
            else
                Clients.Caller.onError("LoadActivityDetail", "輸入參數不正確，請重新確認呼叫參數!!!");

            responseCommonData.Message = "取得活動失敗";

            var badResponse = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<JObject>>(responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;

        }

        /// <summary>
        /// 取得某個模組活動的細項資訊
        /// </summary>
        /// <param name="token"></param>
        /// <param name="moduleKey"></param>
        /// <param name="outerKey"></param>
        public ResultBaseModel<JObject> LoadActivityDetail(Guid token, string moduleKey, string outerKey)
        {
            return LoadActivityDetail(token, moduleKey, outerKey, 10);
        }

        /// <summary>
        /// 初始查詢(取得水道清單列表)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="maxResult"></param>
        /// <param name="outerKeyString"></param>
        public ReadMarkResult<ActivitysViewModel> LoadInitialActivities(Guid token, string circleKey, int maxResult = actMaxResult, string outerKeyString = null)
        {
            var responseCommonData = new ReadMarkResult<ActivitysViewModel>();
            responseCommonData.Success = false;
            responseCommonData.Data = new ActivitysViewModel[0];
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    var eventId = outerKeyString != null ?
                        OuterKeyHelper.CheckOuterKey(outerKeyString) :
                        Guid.Empty;
                    var model = activityService.GetList(circleKey, isMember.MemberId, maxResult, eventId.Value);

                    if (model.Success)
                    {
                        Clients.Caller.showInitActivities(model);
                        responseCommonData.Success = true;
                        responseCommonData = model;
                    }
                    else
                    {
                        responseCommonData.Message = model.Message;
                        Clients.Caller.onError("LoadInitialActivities", model.Message);
                    }
                }
                else
                {
                    responseCommonData.Message = "LoadNewerActivities身分驗證失敗，請重新登入!token:[" + token + "]";
                    Clients.Caller.onError("LoadInitialActivities", "身分驗證失敗，請重新登入!token:[" + token + "]");
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得活動列表發生意外", ex.Message));
                Clients.Caller.onError("LoadInitialActivities", "取得活動列表發生意外: " + msg);
                responseCommonData.Message = "LoadInitialActivities 取得活動列表發生意外:" + msg;
            }
            return responseCommonData;
        }

        /// <summary>
        /// 取得較舊活動
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="maxResult"></param>
        /// <param name="outerKeyString"></param>
        public ReadMarkResult<ActivitysViewModel> LoadOlderActivities(Guid token, string circleKey, int maxResult = actMaxResult, string outerKeyString = null)
        {
            var responseCommonData = new ReadMarkResult<ActivitysViewModel>();
            responseCommonData.Success = false;
            responseCommonData.Data = new ActivitysViewModel[0];
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    var eventId = outerKeyString != null ?
                                             OuterKeyHelper.CheckOuterKey(outerKeyString) :
                                             Guid.Empty;
                    /*if (eventId == null || eventId == Guid.Empty)
                        return;*/

                    var model = activityService.GetListByDirection(true, circleKey, isMember.MemberId, maxResult, eventId.Value);
                    responseCommonData.Success = model.Success;
                    if (model.Success)
                    {
                        responseCommonData = model;
                        if (string.IsNullOrEmpty(outerKeyString))
                            Clients.Caller.showLastActivities(model, true);
                        else
                            Clients.Caller.showOlderActivities(model);
                    }
                    else
                    {
                        responseCommonData.Message = "LoadOlderActivitieserror:[" + model.Message + "]";
                        Clients.Caller.onError("LoadOlderActivities", "error: " + model.Message);
                    }
                }
                else
                {
                    Clients.Caller.onError("LoadOlderActivities", "身分驗證失敗，請重新登入! token:[" + token + "]");
                    responseCommonData.Message = "LoadOlderActivities身分驗證失敗，請重新登入!token:[" + token + "]";
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得較舊活動列表發生意外", ex.Message));
                Clients.Caller.onError("LoadOlderActivities", "取得較舊活動列表發生意外: " + msg);
                responseCommonData.Message = "LoadOlderActivities 取得較舊活動列表發生意外:" + msg;
            }
            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ReadMarkResult<ActivitysViewModel>>
           (responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }

        /// <summary>
        /// 取得較新活動
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="maxResult"></param>
        /// <param name="outerKeyString"></param>
        public ReadMarkResult<ActivitysViewModel> LoadNewerActivities(Guid token, string circleKey, int maxResult = actMaxResult, string outerKeyString = null)
        {
            var responseCommonData = new ReadMarkResult<ActivitysViewModel>();
            responseCommonData.Success = false;
            responseCommonData.Data = new ActivitysViewModel[0];

            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    var eventId = outerKeyString != null ?
                                             OuterKeyHelper.CheckOuterKey(outerKeyString) :
                                             Guid.Empty;
                    /*    if (eventId == null || eventId == Guid.Empty)
                            return;*/

                    var model = activityService.GetListByDirection(false, circleKey, isMember.MemberId, maxResult, eventId.Value);
                    responseCommonData.Success = model.Success;
                    if (model.Success)
                    {
                        responseCommonData = model;
                        Clients.Caller.showNewerActivities(model);
                    }
                    else
                        Clients.Caller.onError("LoadNewerActivities", "error: " + model.Message);
                }
                else {
                    Clients.Caller.onError("LoadNewerActivities", "身分驗證失敗，請重新登入!token:[" + token + "]");
                    responseCommonData.Message = "LoadNewerActivities身分驗證失敗，請重新登入!token:[" + token + "]";
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得較新活動列表發生意外", ex.Message));
                Clients.Caller.onError("LoadNewerActivities", "取得較新活動列表發生意外: " + msg);
                responseCommonData.Message = "LoadNewerActivities 取得較新活動列表發生意外:" + msg;
            }
            var response = new System.Web.Http.Results.OkNegotiatedContentResult<ReadMarkResult<ActivitysViewModel>>
   (responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }

        public ResultBaseModel<ActivitysLatest> LoadRecordList(Guid token)
        {
            // 載入活動紀錄，不特別給現在學習圈代碼
           return  LoadRecordList(token, "");
        }

        public ResultBaseModel<ActivitysLatest> LoadRecordList(Guid token, string circleKey)
        {
            var responseCommonData = new ResultBaseModel<ActivitysLatest>();
            responseCommonData.Success = false;
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    var data = activityService.GetLatestList(isMember.MemberId, circleKey);
                    if (data != null && data.FirstOrDefault() != null)
                    {
                        responseCommonData.Success = true;
                        responseCommonData.Data = data.ToArray();
                        Clients.Caller.showRecordList(data);
                    }
                    else
                    {
                        responseCommonData.Message = "LoadRecordList 取得失敗";
                        Clients.Caller.showRecordList(new string[0]);
                    }
                }
                else
                {
                    responseCommonData.Message = "LoadRecordList 身分驗證失敗，請重新登入!token:[" + token + "]";
                    Clients.Caller.onError("LoadRecordList", " 身分驗證失敗，請重新登入!token:[" + token + "]");
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得紀錄列表發生意外", ex.Message));
                Clients.Caller.onError("LoadRecordList", "取得紀錄列表發生意外: " + msg);
                responseCommonData.Message = "LoadRecordList 取得紀錄列表發生意外" + msg;
            }

           // var response = new System.Web.Http.Results.OkNegotiatedContentResult<ResultBaseModel<ActivitysLatest>>(responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }

        /// <summary>
        /// 刪除訊息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="createtime"></param>
        public BaseResponse<string> DeleteRecord(Guid token, string circleKey, DateTime createtime)
        {
            var response = new BaseResponse<string>();
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {

                    var lastActivityInfo = activityService.GetLastActivity(circleKey.ToLower());
                    if (lastActivityInfo != null)
                    {
                        //若條件日期與該學習圈最後一筆活動的建立日期不一致，代表有新的訊息，因此無法刪除該訊息列
                        if (lastActivityInfo.Publish_Utc.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss") != createtime.ToString("yyyy-MM-ddTHH:mm:ss"))
                        {
                            response.Success = true;
                            response.Message = "因有新的訊息,刪除失敗";
                        }
                        else
                        {
                            var result = activityService.DeleteLatest(isMember.MemberId, circleKey.ToLower());
                            //更新至最新
                            UpdateDeleteRead(token, circleKey);
                            if (result)
                            {
                                activityService.GetLatestList(isMember.MemberId, circleKey.ToLower());
                                response.Success = true;
                                response.Message = "刪除成功";
                                // Clients.Caller.showRecordList(data);
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "查無資訊";
                            }
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "查無資訊";
                    }

                    //       Clients.Caller.showRecordList(false, "查無資訊");
                }
                else
                {
                    response.Success = false;
                    response.Message = "DeleteRecord 身分驗證失敗，請重新登入!token:[" + token + "]";
                }
                //Clients.Caller.onError("LoadRecordList", " 身分驗證失敗，請重新登入!token:[" + token + "]");
                return response;
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                response.Success = false;
                response.Message = "DeleteRecord 刪除紀錄列表發生意外" + msg;
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "DeleteRecord 刪除紀錄列表發生意外", ex.Message));
                return response;
                //  Clients.Caller.onError("LoadRecordList", "取得紀錄列表發生意外: " + msg);
            }
        }

        /// <summary>
        /// 用於呼叫某個ActivityNoticesId之後的筆數;若無ActivityNoticesId,則顯示設定筆數資訊
        /// </summary>
        /// <param name="token"></param>
        /// <param name="maxResult"></param>
        /// <param name="ActivityNoticeId"></param>
        public bool LoadNoticeListById(Guid token, int? maxResult = noticeMaxResult, int? ActivityNoticeId = 0)
        {
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    // var data = new ActivitysNoticeViewModel();
                    var data = new Infrastructure.ViewModel.ActivitysNoticeViewModel();
                    if (ActivityNoticeId == 0)
                        //data = _actService.GetNoticeList(memberId, maxResult.Value);
                        data = noticeService.GetNoticeList("myNotice", isMember.MemberId, 20);
                    else
                        //data = _actService.GetNoticeList(memberId, maxResult.Value, ActivityNoticeId.Value);
                        data = noticeService.GetNoticeList("myNotice", isMember.MemberId, 20, ActivityNoticeId.Value);
                    Clients.Caller.showNoticeListById(data);
                    return true;
                }
                else
                {
                    Clients.Caller.onError("LoadNoticeList", " 身分驗證失敗，請重新登入!token:[" + token + "]");
                    return false;
                }

            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "LoadNoticeList 取得紀錄列表發生意外", ex.Message));
                Clients.Caller.onError("LoadNoticeList", "取得紀錄列表發生意外: " + msg);
                return false;
            }
        }
        public BaseResponse<ActivitysNoticeViewModel> LoadNoticeList(Guid token, int? maxResult = noticeMaxResult)
        {
            var responseCommonData = new BaseResponse<ActivitysNoticeViewModel>();
            responseCommonData.Success = false;
            responseCommonData.Data = new ActivitysNoticeViewModel();
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    //myNotice
                    //  var data = _actService.GetNoticeList(memberId, maxResult.Value);
                    var data = noticeService.GetNoticeList("myNotice", isMember.MemberId, 20);
                    Clients.Caller.showNoticeList(data);
                    responseCommonData.Success = true;
                    responseCommonData.Data = data;
                }
                else
                {
                    responseCommonData.Data = new ActivitysNoticeViewModel();
                    Clients.Caller.onError("LoadNoticeList", " 身分驗證失敗，請重新登入!token:[" + token + "]");
                    //var badResponse = new System.Web.Http.Results.BadRequestResult(new Controllers.APPSupport.EmptyController());
                   return responseCommonData;
                }

            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                responseCommonData.Data = new ActivitysNoticeViewModel();
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "取得紀錄列表發生意外 取得紀錄列表發生意外", msg));
                Clients.Caller.onError("LoadNoticeList", "取得紀錄列表發生意外: " + msg);
                var badResponse = new System.Web.Http.Results.BadRequestResult(new Controllers.APPSupport.EmptyController());
                return responseCommonData;
            }
            var response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<ActivitysNoticeViewModel>>(responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }

        public BaseResponse<string> ClearNoticeUnreadCount(Guid token)
        {
            var responseCommonData = new BaseResponse<string>();
            responseCommonData.Success = false;
            try
            {
                // 是否為合法使用者ClearNoticeUnreadCount
                var isMember = CheckToken(token.ToString());
                if (isMember != null) {
                    activityService.ClearNoticeCount(isMember.MemberId);
                    responseCommonData.Success = true;
                    responseCommonData.Data = "清除成功";
                    responseCommonData.Message = "清除成功";
                }
                else { Clients.Caller.onError("ClearNoticeUnreadCount", " 身分驗證失敗，請重新登入!token:[" + token + "]");
                    responseCommonData.Data = "ClearNoticeUnreadCount身分驗證失敗，請重新登入!token:[" + token + "]";
                    responseCommonData.Message = "ClearNoticeUnreadCount身分驗證失敗，請重新登入!token:[" + token + "]";
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "清除訊息紅點發生意外: token:[" + token + "]", msg));
                Clients.Caller.onError("ClearNoticeUnreadCount", "清除訊息紅點發生意外: token:[" + token + "]" + msg);
                responseCommonData.Data = "ClearNoticeUnreadCount 清除訊息紅點發生意外: token:[" + token + "]" + msg;
                responseCommonData.Message = "ClearNoticeUnreadCount 清除訊息紅點發生意外: token:[" + token + "]" + msg;
            }
            //var response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<string>>(responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }

        public BaseResponse<string> UpdateNoticeClick(Guid token, int id)
        {
            var responseCommonData = new BaseResponse<string>();
            responseCommonData.Success = false;
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    activityService.UpdateNoticeClick(isMember.MemberId, id);
                    responseCommonData.Success = true;
                    responseCommonData.Data = "修改成功";
                }
                else {
                    Clients.Caller.onError("UpdateNoticeClick", " 身分驗證失敗，請重新登入!token:[" + token + "]");
                    responseCommonData.Data = "UpdateNoticeClick 身分驗證失敗，請重新登入!token:[" + token + "]";
                }

            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}:「{1}」", "UpdateNoticeClick 更新訊息按鈕發生意外: token:[" + token + "]", msg));
                Clients.Caller.onError("UpdateNoticeClick", "更新訊息按鈕發生意外: token:[" + token + "]" + msg);
                responseCommonData.Data = "UpdateNoticeClick 更新訊息按鈕發生意外: token:[" + token + "]" + msg;
            }
            var response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<string>>(responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }
        /// <summary>
        /// 查詢課程角色資訊
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circlekey"></param>
        public List<Infrastructure.BusinessObject.LearningCircleMemberList> showMemberRoleLists(Guid token, string circlekey)
        {
           return ShowMemberRoleList(token, circlekey);
        }
        /// <summary>
        /// 取得課程內的使用者角色清單(角色包含使用者)
        /// 2016-11-28 edit by sophiee 因成員UI未確認，保留ShowMemberRoleList方法，以免影響APP程式運作
        /// 另開ShowMemberRoleLists方法，未來必須整併
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        public List<Infrastructure.BusinessObject.LearningCircleMemberList> ShowMemberRoleList(Guid token, string circleKey)
        {
            var responseCommonData = new List<Infrastructure.BusinessObject.LearningCircleMemberList>();
            // 是否為合法使用者
            var isMember = CheckToken(token.ToString());
            if (isMember != null)
            {
                var learningCircleRoleService = new LearningRoleService();
                //取得學習圈成員 
                var list = learningCircleRoleService.GetLearningCircleMemberList(circleKey);
                responseCommonData = list.ToList();
                Clients.Caller.showInitMemberRoleList(list);
            }
            else
            {
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}", "ShowMemberRoleList 身分驗證失敗，請重新登入!token:[" + token + "]"));
                Clients.Caller.onError("ShowMemberRoleList", "身分驗證失敗，請重新登入! token:[" + token + "]");
            }
       /*     var response = new System.Web.Http.Results.OkNegotiatedContentResult<List<Infrastructure.BusinessObject.LearningCircleMemberList>>
                (responseCommonData, new Controllers.APPSupport.EmptyController());*/
            return responseCommonData;

        }

        /// <summary>
        /// 刪除活動
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="moduleKey"></param>
        /// <param name="outerKey"></param>
        public Infrastructure.ViewModel.Base.BaseResponse<string> DeleteActivity(Guid token, string circleKey, string moduleKey, string outerKey)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = false;
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    var eventId = Service.Utility.OuterKeyHelper.CheckOuterKey(outerKey);
                    if (eventId.HasValue == false)
                    {
                        response.Message = "outerKey為錯誤格式";
                        return response;
                    }

                    //取得傳入的參數
                    ModuleParameter param = new ModuleParameter()
                    {
                        ModuleKey = moduleKey,
                        EventId = eventId.Value,
                        MemberId = isMember.MemberId,
                        CircleKey = circleKey
                    };
                    var moduleService = new ModuleService();
                    var responseData = moduleService.DeleteActivity(param);

                    //為了即時停止點名而做的 2017-05-10 育澍
                    if (moduleKey.ToLower() == "signin")
                    {
                        //把活動開始時間改成現在
                        var signInService = new SignInService();
                        var duration = signInService.UpdateStartDate(eventId.Value, isMember.MemberId, false);

                        //告訴同班的有點名活動結束
                        Clients.Group(circleKey.ToLower()).signIn_eventStop(outerKey, duration);
                    }
                    if (moduleKey.ToLower() == "vote")
                        voteChangeStart(token, circleKey, outerKey, false);



                    //有活動被刪除，告知群組內更新活動牆
                    Clients.Group(circleKey.ToLower()).deleteActivity(moduleKey, outerKey);
                    response.Success = true;
                    response.Message = "刪除成功";
                    //更新通知列表
                    ShowNoticeList(circleKey.ToLower());
                }
                else
                {
                    response.Success = false;
                    response.Message = " 身分驗證失敗，請重新登入! token:[" + token + "]";

                    Clients.Caller.onError("DeleteActivity", " 身分驗證失敗，請重新登入! token:[" + token + "]");
                }


                return response;
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}{1}", "DeleteActivity 刪除活動發生意外", msg));

                Clients.Caller.onError("DeleteActivity", "刪除活動發生意外: " + msg);
                return response;
            }
        }

        /// <summary>
        /// 回傳指定活動細節
        /// </summary>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
        public void LoadDetail(Guid token, string outerKey)
        {
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    var eventId = outerKey != null ?
                                             OuterKeyHelper.CheckOuterKey(outerKey) :
                                             Guid.Empty;
                    if (eventId == null || eventId == Guid.Empty)
                        return;
                    #region //判斷模組，取得細節資料

                    var _detail = new ModuleDetailViewModel();

                    var act = activityService.GetByEventId(eventId.Value);
                    if (act != null)
                    {
                        //取得傳入的參數
                        ModuleParameter param = new ModuleParameter()
                        {
                            ModuleKey = act.ModuleKey,
                            EventId = eventId.Value,
                            MemberId = isMember.MemberId
                        };
                        var moduleService = new ModuleService();
                        var detail = moduleService.GetActivityDetail(param);
                        if (detail != null)
                        {
                            Clients.Caller.renderDetails(act.ModuleKey, detail);
                        }
                        else
                            Clients.Caller.onError("LoadDetail", " 模組資料不存在，請確認! ModuleKey:[" + act.ModuleKey + "]");
                    }
                    else
                        Clients.Caller.onError("LoadDetail", " 查無活動，請確認! outerKey:[" + outerKey + "]");
                    #endregion
                }
                else
                    Clients.Caller.onError("LoadDetail", " 身分驗證失敗，請重新登入! token:[" + token + "]");
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}{1}", "LoadDetail 載入活動細節發生意外", msg));

                Clients.Caller.onError("LoadDetail", "載入活動細節發生意外: " + msg);
            }
        }
        public void UpdateDeleteRead(Guid token, string circleKey, string outerKey = null)
        {
            try
            {
                // 是否為合法使用者
                var isMember = CheckToken(token.ToString());
                if (isMember != null)
                {
                    int id = 0;
                    if (outerKey == null || outerKey == string.Empty)
                    {
                        var activityInfo = activityService.GetLastActivity(circleKey);
                        if (activityInfo != null)
                            // 更新已讀至群組最新一筆活動
                            id = activityInfo.Id;
                    }
                    else
                    {
                        // 更新已讀至特定一筆活動
                        var guid = Service.Utility.OuterKeyHelper.CheckOuterKey(outerKey);
                        if (guid.HasValue == false)
                        {
                            Clients.Caller.onError("UpdateRead", "outerKey格式錯誤");
                        }
                        var activityInfo = activityService.GetByEventId(guid.Value);
                        if (activityInfo != null)
                            // 更新已讀至群組最新一筆活動
                            id = activityInfo.Id;
                    }
                    if (id > 0)
                        activityService.UpdateRead(circleKey.ToLower(), isMember.MemberId, id);
                }
                else
                    Clients.Caller.onError("UpdateRead", " 身分驗證失敗，請重新登入! token:[" + token + "]");
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}{1}", "UpdateDeleteRead 更新已讀活動發生意外", msg));

                Clients.Caller.onError("UpdateDeleteRead", "更新已讀活動發生意外: " + msg);
            }
        }


        public BaseResponse<IEnumerable<ActivitysLatest>> UpdateRead(Guid token, string circleKey, string outerKey = null)
        {
            var responseCommonData = new BaseResponse<IEnumerable<ActivitysLatest>>();
            responseCommonData.Success = false;
            responseCommonData.Data = new List<ActivitysLatest>();
            try
            {
                if (circleKey == null || circleKey == string.Empty) {
                    responseCommonData.Message = "UpdateRead 遺漏參數";
                    Clients.Caller.onError("UpdateRead", "遺漏參數");
                }
                else
                {
                    // 是否為合法使用者
                    var isMember = CheckToken(token.ToString());
                    if (isMember != null)
                    {
                        int id = 0;
                        if (string.IsNullOrEmpty(outerKey))
                        {
                            var activityInfo = activityService.GetLastActivity(circleKey);
                            if (activityInfo != null)
                                // 更新已讀至群組最新一筆活動
                                id = activityInfo.Id;
                        }
                        else
                        {
                            // 更新已讀至特定一筆活動
                            var guid = Service.Utility.OuterKeyHelper.PageTokenToGuid(outerKey);
                            var activityInfo = activityService.GetByEventId(guid);
                            if (activityInfo != null)
                                // 更新已讀至群組最新一筆活動
                                id = activityInfo.Id;
                        }
                        if (id > 0)
                            activityService.UpdateRead(circleKey.ToLower(), isMember.MemberId, id);
   
                        var datas=LoadRecordList(token, circleKey);
                        if (datas.Data.FirstOrDefault() != null)
                        {
                            responseCommonData.Success = true;
                            responseCommonData.Data = datas.Data.ToList();
                        }

                    }
                    else
                    {
                        responseCommonData.Message = "UpdateRead 身分驗證失敗，請重新登入! token:[" + token + "]";
                        Clients.Caller.onError("UpdateRead", " 身分驗證失敗，請重新登入! token:[" + token + "]");
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                errorService.InsertError((int)SystemErrorTypeNum.SignalRError, string.Format("{0}{1}", "UpdateRead 更新已讀活動發生意外", msg));
                Clients.Caller.onError("UpdateRead", "更新已讀發生意外: " + msg);
                responseCommonData.Message = "UpdateRead 更新已讀發生意外: " + msg;
            }
            var response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<IEnumerable<ActivitysLatest>>>
             (responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }



        private void ShowRecordList(string circleKey)
        {

            var learningCircleService = new LearningCircleService();
            var members = learningCircleService.GetCircleMemberIdList(circleKey);
            foreach (var mem in members)
            {
                var myConn = HttpContext.Current.Cache.Get(mem.ToString()) as List<string>;
                if (myConn != null)
                {
                    var data = activityService.GetLatestList(mem, circleKey);

                    // 所有歸屬在這個id下的connection都會收到
                    foreach (var connId in myConn)
                    {
                        Clients.Client(connId).showRecordList(data);
                    }
                }
            }
        }

        private void ShowNoticeList(string circleKey)
        {
            var learningCircleService = new LearningCircleService();

            var members = learningCircleService.GetCircleMemberIdList(circleKey);
            foreach (var mem in members)
            {
                var myConn = HttpContext.Current.Cache.Get(mem.ToString()) as List<string>;
                if (myConn != null)
                {
                    var data = activityService.GetNoticeList(mem, noticeMaxResult);

                    // 所有歸屬在這個id下的connection都會收到
                    foreach (var connId in myConn)
                    {
                        Clients.Client(connId).showNoticeList(data);
                    }
                }
            }
        }

        private void AddNotice(string circleKey, Guid eventId, int memberId, string message)
        {
            // 將通知寫入資料庫
            var data = activityService.AddNotice(circleKey, memberId, eventId, message);
            if (HttpContext.Current == null)
                return;
            // signalr發送通知
            var myConn = HttpContext.Current.Cache.Get(memberId.ToString()) as List<string>;
            if (myConn != null)
            {
                // 所有歸屬在這個id下的connection都會收到
                foreach (var connId in myConn)
                {
                    Clients.Client(connId).showNoticeList(data);
                    Clients.Client(connId).appendNotice(data);
                }
            }

        }
    }
}

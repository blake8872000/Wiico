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
using WiicoApi.Controllers.Common;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;
using WiicoApi.SignalR;
using WiicoApi.SignalRHub;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.Controllers.APPSupport
{
    /// <summary>
    /// 主題討論API
    /// </summary>
    public class DiscussionController : MultipartFormDataFilesController<DiscussionCreateRequestModel>
    {
        /// <summary>
        /// 檔案處理服務
        /// </summary>
        private CacheService cacheService = null;
        private DiscussionService discussionService = null;
        private DiscussionFuncFile discussionFileService = null;
        private TokenService tokenService = new TokenService();
        private ActivityService activityService = null;
        private NoticeService noticeService = null;
        private AuthService authService = null;
        private LearningCircleService learingCircleService = null;
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();
        /// <summary>
        /// 預設縮圖最大寬度
        /// </summary>       
        private readonly int maxImgWidth = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgWidth"]);
        /// <summary>
        /// 預設縮圖最大高度
        /// </summary>
        private readonly int maxImgHeight = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgHeight"]);
        private IHubContext objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoHub>();

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string outerKey, int? maxresult = 10)
        {
            discussionService = new DiscussionService();
            var result = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionDetail>();
            var data = discussionService.GetDetailByOuterKey(outerKey, maxresult);
            if (data != null)
            {
                result.Success = true;
                result.Data = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionDetail[1] { data };
                result.Message = "查詢成功";
            }
            else
            {
                result.Success = false;
                result.Message = "查詢失敗";
            }
            return Ok(result);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> POST()
        {
            discussionService = new DiscussionService();
            var result = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            result.Success = false;
            result.Data = "";
            await SetFileData();
            if (multipartFormDataStreamProvider.FormData == null) {
                result.Message = "遺漏參數";
                result.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content( HttpStatusCode.BadRequest, result);
            }
            //設定參數
            var token = multipartFormDataStreamProvider.FormData.Get("ICanToken")!=null ? multipartFormDataStreamProvider.FormData.GetValues("ICanToken")[0] : null;
            var circleKey = multipartFormDataStreamProvider.FormData.Get("ClassID") != null ? multipartFormDataStreamProvider.FormData.GetValues("ClassID")[0] : null;
            var title = multipartFormDataStreamProvider.FormData.Get("Title") != null ? multipartFormDataStreamProvider.FormData.GetValues("Title")[0] : null;
            var content = multipartFormDataStreamProvider.FormData.Get("Content") != null ? multipartFormDataStreamProvider.FormData.GetValues("Content")[0] : null;

            if (token == null || circleKey == null || title == null)
            {
                result.Message = "遺漏參數";
                result.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, result);
            }
            multipartFormDataModel = new DiscussionCreateRequestModel();
            multipartFormDataModel.Token = Guid.Parse(token);
            multipartFormDataModel.CircleKey = circleKey.ToLower();
            multipartFormDataModel.Title = title;
            multipartFormDataModel.Content = content;
            var activityOuterKey = streamProvider.FormData.Get("activityOuterKey") != null ? streamProvider.FormData.GetValues("activityOuterKey")[0] : null;
            var commentOuterKey = streamProvider.FormData.Get("commentOuterKey") != null ? streamProvider.FormData.GetValues("commentOuterKey")[0] : null;
            var replyOuterKey = streamProvider.FormData.Get("replyOuterKey") != null ? streamProvider.FormData.GetValues("replyOuterKey")[0] : null;
            var msg = streamProvider.FormData.Get("msg") != null ? streamProvider.FormData.GetValues("msg")[0] : null;
            multipartFormDataModel.ActivityOuterKey = activityOuterKey != null ? activityOuterKey : null;
            multipartFormDataModel.CommentOuterKey = commentOuterKey != null ? commentOuterKey : null;
            multipartFormDataModel.ReplyOuterKey = replyOuterKey != null ? replyOuterKey : null;
            multipartFormDataModel.Msg = msg != null ? msg : null;

            //驗證token
            var checkMember = tokenService.GetTokenInfo(multipartFormDataModel.Token.ToString()).Result;
            if (checkMember != null)
            {
                authService = new AuthService();
                discussionFileService = new DiscussionFuncFile();
                cacheService = new CacheService();
                learingCircleService = new LearningCircleService();
                var learningCircleInfo = learingCircleService.GetCourseLearningInfo(multipartFormDataModel.CircleKey);
                var hasAuth = authService.CheckFunctionAuth(learningCircleInfo.Id, DiscussionFunction.Manage, checkMember.MemberId);
                //老師新增主題討論活動
                if (hasAuth)
                {
                    var newDiscussion = new Infrastructure.Entity.ActDiscussion();
                    //建立有附加檔案的主題討論
                    if (fileStreams.Count > 0)
                    {
                        var fileService = new FileService();
                        var files = fileService.UploadFiles(checkMember.MemberId, multipartFormDataFiles, fileStreams.ToArray());
                        newDiscussion = discussionService.Add(multipartFormDataModel.CircleKey, checkMember.MemberId, multipartFormDataModel.Title, multipartFormDataModel.Content, files.Count(), 1);
                        //建立主題討論與檔案的關聯
                        var discussionFileReference = discussionFileService.DiscussionFileReference(newDiscussion.Id, files, null);
                    }
                    else
                        newDiscussion = discussionService.Add(multipartFormDataModel.CircleKey, checkMember.MemberId, multipartFormDataModel.Title, multipartFormDataModel.Content, 0, 1);
                    var rtn = new Infrastructure.ViewModel.ActivitysViewModel();
                    if (newDiscussion != null)
                    {
                        result.Success = true;
                        result.Message = "成功建立主題討論活動";
                        activityService = new ActivityService();
                        //顯示回傳app資訊
                        rtn = activityService.SignalrResponse(multipartFormDataModel.CircleKey, newDiscussion.EventId, ModuleType.Discussion, checkMember.MemberId, true);
                        //新增主題討論活動
                        var objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoHub>();
                        objHub.Clients.Group(multipartFormDataModel.CircleKey.ToLower()).appendActivity(rtn, "");
                        //為了推播與發通知查詢學習圈成員
                        var members = cacheService.GetCircleMember(multipartFormDataModel.CircleKey);
                        if(members==null)
                            return Ok(result); 
                        var signalrService = new SignalrService();
                        //發通知
                        var connectIdAndData = signalrService.GetConnectIdAndData(multipartFormDataModel.CircleKey, checkMember.MemberId, SignalrConnectIdType.All, SignalrDataType.Activity);
                        SignalrClientHelper.ShowRecordListById(connectIdAndData);
                        //發通知
                        //  ShowRecordList(members, discussionData.CircleKey);
                        // 推播
                        PushDiscussionOnCreatedAsync(members, multipartFormDataModel.CircleKey, rtn.OuterKey, multipartFormDataModel.Title, checkMember.MemberId);
                    }
                    else
                    {
                        rtn = null;
                        result.Success = false;
                        result.Message = "新增失敗";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "沒有權限";
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// 編輯主題討論
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async  Task<IHttpActionResult> Put()
        {
            discussionService = new DiscussionService();
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();

            response.Success = false;
            response.Data = "";
            await SetFileData();
            if (multipartFormDataStreamProvider.FormData == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            //設定參數
            var token = multipartFormDataStreamProvider.FormData.Get("ICanToken") != null ? multipartFormDataStreamProvider.FormData.GetValues("ICanToken")[0] : null;
            var circleKey = multipartFormDataStreamProvider.FormData.Get("ClassID") != null ? multipartFormDataStreamProvider.FormData.GetValues("ClassID")[0] : null;
            var title = multipartFormDataStreamProvider.FormData.Get("Title") != null ? multipartFormDataStreamProvider.FormData.GetValues("Title")[0] : null;
            var content = multipartFormDataStreamProvider.FormData.Get("Content") != null ? multipartFormDataStreamProvider.FormData.GetValues("Content")[0] : null;
            var activityOuterKey = multipartFormDataStreamProvider.FormData.Get("activityOuterKey") != null ? multipartFormDataStreamProvider.FormData.GetValues("activityOuterKey")[0] : null;
            var removeFiles = multipartFormDataStreamProvider.FormData.Get("removeFiles") != null ? multipartFormDataStreamProvider.FormData.GetValues("removeFiles")[0] : null;

            if (token == null || circleKey == null || title == null || activityOuterKey==null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            multipartFormDataModel = new  DiscussionCreateRequestModel();
            multipartFormDataModel.Token = Guid.Parse(token);
            multipartFormDataModel.CircleKey = circleKey.ToLower();
            multipartFormDataModel.Title = title;
            multipartFormDataModel.Content = content;

           var commentOuterKey = multipartFormDataStreamProvider.FormData.Get("commentOuterKey") != null ? multipartFormDataStreamProvider.FormData.GetValues("commentOuterKey")[0] : null;
            var replyOuterKey = multipartFormDataStreamProvider.FormData.Get("replyOuterKey") != null ? multipartFormDataStreamProvider.FormData.GetValues("replyOuterKey")[0] : null;
            var msg = multipartFormDataStreamProvider.FormData.Get("msg") != null ? multipartFormDataStreamProvider.FormData.GetValues("msg")[0] : null;

            multipartFormDataModel.ActivityOuterKey = activityOuterKey != null ? activityOuterKey : null;
            multipartFormDataModel.CommentOuterKey = commentOuterKey != null ? commentOuterKey : null;
            multipartFormDataModel.ReplyOuterKey = replyOuterKey != null ? replyOuterKey : null;
            multipartFormDataModel.Msg = msg != null ? msg : null;
            
            //驗證token
            var checkMember = tokenService.GetTokenInfo(multipartFormDataModel.Token.ToString()).Result;

            if (checkMember != null && activityOuterKey != null)
            {
                authService = new AuthService();
                learingCircleService = new LearningCircleService();
                var learningCircleInfo = learingCircleService.GetCourseLearningInfo(multipartFormDataModel.CircleKey);
                var hasAuth = authService.CheckFunctionAuth(learningCircleInfo.Id, DiscussionFunction.Manage, checkMember.MemberId);

                //老師編輯主題討論活動
                if (hasAuth)
                {
                    discussionFileService = new DiscussionFuncFile();
                    var updateDiscussion = new Infrastructure.Entity.ActDiscussion();
                    if (multipartFormDataFiles.Count > 0)
                    {
                        var fileService = new FileService();
                        var uploadFileInfo = fileService.UploadFiles(checkMember.MemberId, multipartFormDataFiles, fileStreams.ToArray());
                        updateDiscussion = discussionService.Update(multipartFormDataModel, checkMember.MemberId, uploadFileInfo.Count);
                        //建立主題討論與檔案的關聯
                        var discussionFileReference = discussionFileService.DiscussionFileReference(updateDiscussion.Id, uploadFileInfo, null);
                    }
                    else
                        updateDiscussion = discussionService.Update(multipartFormDataModel, checkMember.MemberId, 0);

                    if (updateDiscussion != null)
                    {
                        cacheService = new CacheService();
                        //刪除檔案
                        if (removeFiles !=null &&removeFiles != string.Empty)
                        {
                            var removeFilesArray = removeFiles.Split(',');
                            //刪除主題討論與檔案的關聯
                            discussionFileService.DeleteDiscussionFileReference(updateDiscussion.Id, removeFilesArray);
                        }
                        var rtn = discussionService.GetUpdateDetailByEventId(updateDiscussion.EventId);
                        response.Success = true;
                        response.Message = "成功編輯主題討論活動";
                        objHub.Clients.Group(multipartFormDataModel.CircleKey.ToLower()).updateDetail("discussion", rtn);
                        //為了推播與發通知查詢學習圈成員
                        var members = cacheService.GetCircleMember(multipartFormDataModel.CircleKey);
                        //為了單元測試用
                        if (members == null)
                            return Ok(response);
                        // 推播
                        PushDiscussionOnUpdateAsync(members, multipartFormDataModel.CircleKey, updateDiscussion.EventId, multipartFormDataModel.Title, checkMember.MemberId);

                        var signalrService = new SignalrService();
                        //發通知
                        var connectIdAndData = signalrService.GetConnectIdAndData(multipartFormDataModel.CircleKey, checkMember.MemberId, SignalrConnectIdType.All, SignalrDataType.Activity);
                        SignalrClientHelper.ShowRecordListById(connectIdAndData);
                        //發通知
                        //ShowRecordList(members, discussionData.CircleKey);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "編輯失敗";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "沒有權限";
                }
            }
            return Ok(response);
        }

        /// <summary>
        /// 新增主題討論活動推播
        /// </summary>
        /// <param name="members"></param>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        private async Task PushDiscussionOnCreatedAsync(List<Infrastructure.BusinessObject.MemberCacheData> members, string circleKey, Guid eventId, string eventName, int myId)
        {
            //刪除自己
            members.Remove(members.FirstOrDefault(t => t.Id == myId));
            var memberService = new MemberService();

            var creator = memberService.UserIdToAccount(myId);
            var eventMessage = string.Format("{1}新增了主題討論：{0}", eventName, creator.Name);
            var pushService = new PushService();
            if (members.Count() > 0)
                await pushService.PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看新的-主題討論", members.Select(t => t.Account).ToArray(), eventMessage);
        }

        /// <summary>
        /// 編輯主題討論活動推播
        /// </summary>
        /// <param name="members"></param>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        private async Task PushDiscussionOnUpdateAsync(List<Infrastructure.BusinessObject.MemberCacheData> members, string circleKey, Guid eventId, string eventName, int myId)
        {
            noticeService = new NoticeService();
            //刪除自己
            members.Remove(members.FirstOrDefault(t => t.Id == myId));
            var memberService = new MemberService();
            var creator = memberService.UserIdToAccount(myId);
            var eventMessage = string.Format("[{0}]內容已更新", eventName);
            var noticeMsg = string.Format("{0} 更新了主題討論 : 「{1} 」", creator.Name, eventName);
            //新增多筆訊息資料
            noticeService.AddMultipleNotice(members, eventId, circleKey, noticeMsg);
            var pushService = new PushService();
            //廣播訊息資訊
            SendNotice(members, circleKey);
            if (members.Count() > 0)
                //推播
                await pushService.PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看更新的主題討論", members.Select(t => t.Account).ToArray(), eventMessage);
        }


        /// <summary>
        /// 廣播發送訊息
        /// </summary>
        /// <param name="members">廣播對象</param>
        /// <param name="circleKey">學習圈代碼</param>
        private void SendNotice(List<Infrastructure.BusinessObject.MemberCacheData> members, string circleKey)
        {
            noticeService = new NoticeService();
            foreach (var member in members)
            {
                // signalr發送通知
                var myConn = System.Web.HttpContext.Current.Cache.Get(member.Id.ToString()) as List<string>;
                if (myConn != null)
                {
                    var data = noticeService.GetNoticeList(circleKey, member.Id, 1);
                    objHub.Clients.Client(myConn.FirstOrDefault()).showNoticeList(data);
                    objHub.Clients.Client(myConn.FirstOrDefault()).appendNotice(data);
                }
            }
        }
    }
}

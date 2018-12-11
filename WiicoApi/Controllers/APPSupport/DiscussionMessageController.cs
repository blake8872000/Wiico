using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;

namespace WiicoApi.Controllers.APPSupport
{
    public class DiscussionMessageController : ApiController
    {
        private DiscussionFuncMsg discussionMsgService = null;
        private NoticeService noticeService = null;
        private MemberService memberService = null;
        private IHubContext objHub = null;
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
        /// <summary>
        /// 取得主題留言的資訊
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="activityOuterKey"></param>
        /// <param name="commentOuterKey"></param>
        /// <param name="maxResult"></param>
        /// <param name="isNewer"></param>
        /// <returns></returns>
        [HttpGet]
        public Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionLoadComment> Get(Guid token, string circleKey, string activityOuterKey, string commentOuterKey, int maxResult = 10, bool? isNewer = true)
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionLoadComment>();
            response.Success = false;
            response.Data = new DiscussionLoadComment[0];
            try
            {
                //訊息列表
                var msgData = new List<DiscussionMessage>();
                //回應資訊
                var resultData = new DiscussionLoadComment();
                memberService = new MemberService();
                // 是否為合法使用者
                var checkMember = memberService.TokenToMember(token);
                discussionMsgService = new DiscussionFuncMsg();

                if (checkMember != null)
                {
                    
                    var replyOuterKey = Guid.Empty;
                    var eventId = Service.Utility.OuterKeyHelper.CheckOuterKey(activityOuterKey);
                    if (eventId.HasValue == false)
                    {
                        response.Message = "outerKey格式錯誤";
                        return response;
                    }
                    //如果要查中間
                    if (commentOuterKey != null)
                    {
                        var replyEventId = Service.Utility.OuterKeyHelper.CheckOuterKey(activityOuterKey);
                        if (replyEventId.HasValue == false)
                        {
                            response.Message = "outerKey格式錯誤";
                            return response;
                        }
                        replyOuterKey = replyEventId.Value;
                        resultData.OuterKey = commentOuterKey;
                        msgData = discussionMsgService.GetMessageList(eventId.Value, replyOuterKey, isNewer).OrderByDescending(t => t.CreateTime).Take(maxResult).Reverse().ToList();
                    }
                    //查全部
                    else
                    {
                        msgData = discussionMsgService.GetMessageList(eventId.Value, null, isNewer).OrderBy(t => t.CreateTime).ToList();
                        resultData.OuterKey = activityOuterKey;
                    }
                    if (msgData != null)
                    {
                        resultData.Comments = msgData;
                        response.Success = true;
                        response.Message = "查詢成功";
                        response.Data = new DiscussionLoadComment[1] { resultData };
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "查詢失敗";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                response.Success = false;
                response.Message = "取得留言發生意外: " + msg;
                return response;
            }
        }
        /// <summary>
        /// 建立新的留言資訊
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Post()
        {
            var result = new Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionSendMsg>();
            var requestFormData = HttpContext.Current.Request.Form;
            var actModuleMessageService = new MessageService();
            var discussionservice = new DiscussionService();
            #region 將request值塞入model中
            var msgData = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionSendMsgRequestModel()
            {
                Token = Guid.Parse(requestFormData["token"].ToString()),
                CircleKey = requestFormData["circleKey"].ToString(),
                ActivityOuterKey = requestFormData["activityOuterKey"].ToString()
            };

            if (requestFormData["commentOuterKey"] != null)
                msgData.CommentOuterKey = requestFormData["commentOuterKey"].ToString();

            if (requestFormData["replyOuterKey"] != null)
                msgData.ReplyOuterKey = requestFormData["replyOuterKey"].ToString();

            if (requestFormData["msg"] != null)
                msgData.Msg = HttpUtility.UrlDecode(requestFormData["msg"].ToString());
            #endregion
            var msgFiles = HttpContext.Current.Request.Files;
            memberService = new MemberService();
            //驗證token
            var checkMember = memberService.TokenToMember(msgData.Token).Result;
            if (checkMember != null)
            {

                //查看主題討論是否存在
                var discussionDBData = discussionservice.GetDBDiscussionInfo(msgData.ActivityOuterKey);
                if (discussionDBData != null)
                {
                    discussionMsgService = new DiscussionFuncMsg();
                    var newMsg = discussionMsgService.AddMessage(msgData.CircleKey, msgData.Msg, checkMember.Id, msgData.ActivityOuterKey, msgData.CommentOuterKey, msgData.ReplyOuterKey);
                    //新增成功
                    if (newMsg != null)
                    {
                        //是否有附加檔案
                        if (msgFiles.Count > 0)
                        {
                            var fileService = new FileService();
                            var discussionFileService = new DiscussionFuncFile();
                            //  var files = await MutipartFileProxy(checkMember);
                            var uploadFileInfo = fileService.UploadFile(checkMember.Id, msgFiles);
                            newMsg.Comment.Photos = uploadFileInfo;
                            //建立主題討論與檔案的關聯
                            var msgFileReference = discussionFileService.DiscussionFileReference(discussionDBData.Id, uploadFileInfo, newMsg.Id);
                        }
                        result.Success = true;
                        result.Message = "成功建立留言";
                        objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoApi.SignalRHub.WiicoHub>();
                        objHub.Clients.Group(msgData.CircleKey.ToLower()).appendComment(newMsg);
                        result.Data = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionSendMsg[1] { newMsg };
                        //推播給回覆者訊息
                        if (newMsg.Comment.Parent.HasValue)
                        {
                            //查出被回覆訊息資訊
                            var replyMsgInfo = actModuleMessageService.GetMsgDBInfoByOuterKey(newMsg.CommentOuterkey);
                            memberService = new MemberService();
                            //查出被回覆者資訊
                            var replyMemberInfo = memberService.UserIdToAccount(replyMsgInfo.CreateUser.Value);

                            if (newMsg.Comment.ReplyOuterKey == null)
                                //推播給被回覆者
                                PushMsgReplyAsync(msgData.CircleKey, newMsg.Comment.EventId, discussionDBData.Name, checkMember, replyMemberInfo, false);
                            else
                                PushMsgReplyAsync(msgData.CircleKey, newMsg.Comment.EventId, discussionDBData.Name, checkMember, replyMemberInfo, true);
                        }
                        //推播給學習圈內所有人
                        else
                            PushDiscussionSendMsgAsync(msgData.CircleKey, newMsg.Comment.EventId, discussionDBData.Name, checkMember);
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "建立留言失敗";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "主題討論不存在";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "驗證身分失敗";
            }
            return Ok(result);
        }

        /// <summary>
        /// 主題討論留言推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="replyMemberInfo"></param>
        /// <returns></returns>
        [HttpPost]
        private async Task PushDiscussionSendMsgAsync(string circleKey, Guid eventId, string eventName, Infrastructure.Entity.Member replyMemberInfo)
        {
            discussionMsgService = new DiscussionFuncMsg();
            var eventMessage = string.Format("{0}回應了一則主題討論", replyMemberInfo.Name);
            var noticeMsg = string.Format("{0}回應了一則主題討論：「 {1}」", replyMemberInfo.Name, eventName);
            var replyMembers = discussionMsgService.GetReplyMemberList(eventId, replyMemberInfo).ToList();
            noticeService = new NoticeService();
            //新增多筆訊息資料
            noticeService.AddMultipleNotice(replyMembers, eventId, circleKey, noticeMsg);
            //廣播訊息資訊
            SendNotice(replyMembers, circleKey);
            var pushService = new PushService();
            var pushMembers = replyMembers.Select(t => t.Account).ToArray();
            if (replyMembers.Count > 0)
                await pushService.PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-被留言", pushMembers, eventMessage);
        }

        /// <summary>
        /// 某個留言被回覆的推播
        /// </summary>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="eventId">主題討論代碼</param>
        /// <param name="eventName">主題討論名稱</param>
        /// <param name="replyMember">回覆者資訊</param>
        /// <param name="beReplyMemberInfo">被回覆的人資訊</param>
        /// <param name="isTag">是否為回覆</param>
        /// <returns></returns>
        private async Task PushMsgReplyAsync(string circleKey, Guid eventId, string eventName, Infrastructure.Entity.Member replyMember, Infrastructure.Entity.Member beReplyMemberInfo, bool? isTag = false)
        {
            var eventMessage = isTag.Value ? (string.Format("{0}回應了一則主題討論的回覆", replyMember.Name)) : (string.Format("{0}回應了一則主題討論的留言", replyMember.Name));
            var noticeMsg = isTag.Value ? string.Format("{0}回應了一則主題討論的回覆：「{1}」", replyMember.Name, eventName) : string.Format("{0}回應了一則主題討論的留言：「{1}」", replyMember.Name, eventName);
            discussionMsgService = new DiscussionFuncMsg();
            // var pushMember =replyMember.Id!= beReplyMemberInfo.Id ? new string[1] { beReplyMemberInfo.Account } : new string[0];
            var replyMembers = discussionMsgService.GetReplyMemberList(eventId, replyMember).ToList();
            if (replyMembers.Count() > 0)
            {
                noticeService = new NoticeService();
                //新增多筆訊息資料
                noticeService.AddMultipleNotice(replyMembers, eventId, circleKey, noticeMsg);
                //廣播訊息資訊
                SendNotice(replyMembers, circleKey);
                var pushService = new PushService();
                if (isTag.Value)
                    await pushService.PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-回覆被回覆", replyMembers.Select(t => t.Account).ToArray(), eventMessage);
                else
                    await pushService.PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-留言被回覆", replyMembers.Select(t => t.Account).ToArray(), eventMessage);
            }
        }

        /// <summary>
        /// 廣播發送訊息
        /// </summary>
        /// <param name="members">廣播對象</param>
        /// <param name="circleKey">學習圈代碼</param>
        private void SendNotice(List<Infrastructure.BusinessObject.MemberCacheData> members, string circleKey)
        {
            noticeService = new NoticeService();
            objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoApi.SignalRHub.WiicoHub>();
            foreach (var member in members)
            {
                // signalr發送通知
                var myConn = System.Web.HttpContext.Current.Cache.Get(member.Id.ToString()) as List<string>;
                if (myConn != null)
                {
                    var data = noticeService.GetNoticeList(circleKey, member.Id, 1);
                    foreach (var connId in myConn)
                    {
                        objHub.Clients.Client(connId).showRecordList(data);
                        objHub.Clients.Client(connId).appendNotice(data);
                    }
                }
            }
        }

    }
}


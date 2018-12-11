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
using System.Web.Http.Cors;
using WiicoApi.Controllers.Common;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 建立新的主題討論訊息
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class AddDiscussionMessageController : MultipartFormDataFilesController<DiscussionSendMsgRequestModel>
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
        /// 建立新的留言資訊
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Post()
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionSendMsg>();

            var actModuleMessageService = new MessageService();
            var discussionservice = new DiscussionService();

            #region 將request值塞入model中

            await SetFileData();
            if (multipartFormDataStreamProvider.FormData == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
       
            //設定參數
            var token = multipartFormDataStreamProvider.FormData.Get("iCanToken") != null ? multipartFormDataStreamProvider.FormData.GetValues("iCanToken")[0] : null;
            var circleKey = multipartFormDataStreamProvider.FormData.Get("classId") != null ? multipartFormDataStreamProvider.FormData.GetValues("classId")[0] : null;
            var activityOuterKey = multipartFormDataStreamProvider.FormData.Get("activityOuterKey") != null ? multipartFormDataStreamProvider.FormData.GetValues("activityOuterKey")[0] : null;
            var msg = multipartFormDataStreamProvider.FormData.Get("msg") != null ? multipartFormDataStreamProvider.FormData.GetValues("msg")[0] : null;

            var commentOuterKey= multipartFormDataStreamProvider.FormData.Get("commentOuterKey") != null ? multipartFormDataStreamProvider.FormData.GetValues("commentOuterKey")[0] : null;
            var replyOuterKey =multipartFormDataStreamProvider.FormData.Get("replyOuterKey") != null ? multipartFormDataStreamProvider.FormData.GetValues("replyOuterKey")[0] : null;

            token =(token==null && multipartFormDataStreamProvider.FormData.Get("token") != null) ?
                multipartFormDataStreamProvider.FormData.GetValues("token")[0] : token;

            circleKey=(circleKey==null && multipartFormDataStreamProvider.FormData.Get("circleKey") != null) ?
                multipartFormDataStreamProvider.FormData.GetValues("circleKey")[0] : circleKey;

            multipartFormDataModel = new DiscussionSendMsgRequestModel() {
                ActivityOuterKey=activityOuterKey,
                CircleKey=circleKey,
                CommentOuterKey=commentOuterKey,
                Msg=msg,
                ReplyOuterKey=replyOuterKey,
                Token=Guid.Parse(token)
            };

            #endregion

            memberService = new MemberService();
            //驗證token
            var checkMember = memberService.TokenToMember(multipartFormDataModel.Token).Result;
            if (checkMember != null)
            {

                //查看主題討論是否存在
                var discussionDBData = discussionservice.GetDBDiscussionInfo(multipartFormDataModel.ActivityOuterKey);
                if (discussionDBData != null)
                {
                    discussionMsgService = new DiscussionFuncMsg();
                    var newMsg = discussionMsgService.AddMessage(multipartFormDataModel.CircleKey, multipartFormDataModel.Msg, checkMember.Id, multipartFormDataModel.ActivityOuterKey, multipartFormDataModel.CommentOuterKey, multipartFormDataModel.ReplyOuterKey);
                    //新增成功
                    if (newMsg != null)
                    {
                        //是否有附加檔案
                        if (fileStreams != null && fileStreams.Count > 0)
                        {
                            var fileService = new FileService();
                            var discussionFileService = new DiscussionFuncFile();
                            //  var files = await MutipartFileProxy(checkMember);
                            var uploadFileInfo = fileService.UploadFiles(checkMember.Id, multipartFormDataFiles, fileStreams.ToArray());
                            newMsg.Comment.Photos = uploadFileInfo;
                            //建立主題討論與檔案的關聯
                            var msgFileReference = discussionFileService.DiscussionFileReference(discussionDBData.Id, uploadFileInfo, newMsg.Id);
                        }
                        response.Success = true;
                        response.Message = "成功建立留言";
                        objHub = GlobalHost.ConnectionManager.GetHubContext<SignalRHub.WiicoHub>();
                        objHub.Clients.Group(multipartFormDataModel.CircleKey.ToLower()).appendComment(newMsg);
                        response.Data = new DiscussionSendMsg[1] { newMsg };
                        //推播給回覆者訊息
                        if (newMsg.Comment.Parent.HasValue)
                        {
                            //查出被回覆訊息資訊
                            var replyMsgInfo = actModuleMessageService.GetMsgDBInfoByOuterKey(newMsg.CommentOuterkey);
                            memberService = new MemberService();
                            //查出被回覆者資訊
                            // var replyMemberInfo = memberService.GetMemberInfo(replyMsgInfo.CreateUser.Value);
                            var replyMemberInfo = memberService.UserIdToAccount(replyMsgInfo.CreateUser.Value);
                            if (newMsg.Comment.ReplyOuterKey == null)
                                //推播給被回覆者
                                PushMsgReplyAsync(multipartFormDataModel.CircleKey, newMsg.Comment.EventId, discussionDBData.Name, checkMember, replyMemberInfo, false);
                            else
                                PushMsgReplyAsync(multipartFormDataModel.CircleKey, newMsg.Comment.EventId, discussionDBData.Name, checkMember, replyMemberInfo, true);
                        }
                        //推播給學習圈內所有人
                        else
                            PushDiscussionSendMsgAsync(multipartFormDataModel.CircleKey, newMsg.Comment.EventId, discussionDBData.Name, checkMember);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "建立留言失敗";
                        response.State = Infrastructure.ViewModel.Base.LogState.Error;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "主題討論不存在";
                    response.State = Infrastructure.ViewModel.Base.LogState.Error;
                }
            }
            else
            {
                response.Success = false;
                response.Message = "驗證身分失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
            }
            return Ok(response);
        }

        /// <summary>
        /// 主題討論留言推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="replyMemberInfo"></param>
        /// <returns></returns>
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
            var pushMembers = replyMembers.Select(t => t.Account).ToArray();
            var pushService = new PushService();
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
            objHub = GlobalHost.ConnectionManager.GetHubContext<SignalRHub.WiicoHub>();
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
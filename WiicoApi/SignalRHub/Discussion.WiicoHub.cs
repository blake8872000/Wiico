using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;
using WiicoApi.SignalR;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.SignalRHub
{
    public partial class WiicoHub : WiicoHubBase
    {
        private DiscussionService discussionService = new DiscussionService();
        private DiscussionFuncMsg discussionMsgService = new DiscussionFuncMsg();
        private DiscussionFuncLike discussionLikeService = new DiscussionFuncLike();
        #region 新的主題討論SignalR方法
        /// <summary>
        /// 進入留言內頁
        /// </summary>
        /// <param name="token">使用者代碼</param>
        /// <param name="outerKey">留言代碼</param>
        /// <param name="maxResult"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionCommentDetail> LoadCommentDetail(Guid token, string outerKey, int? maxResult = 10)
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionCommentDetail>();
            response.Success = false;
            response.Data = new DiscussionCommentDetail[0];
            var checkToken = CheckToken(token.ToString());

            if (checkToken == null)
                response.Message = "身分驗證失敗";
            var data = discussionMsgService.GetCommentDetail(token, outerKey, maxResult);
            if (data != null)
            {
                response.Data = new DiscussionCommentDetail[1] { data };
                response.Success = true;
                response.Message = "查詢成功";
                // 將訊息發送給群組
                Clients.Caller.loadCommentDetail(data);
            }
            else
            {
                response.Success = false;
                response.Message = "查詢失敗";
            }
            return response;
        }

        /// <summary>
        ///  取得新的資訊
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="activityOuterKey"></param>
        /// <param name="commentOuterKey"></param>
        /// <param name="maxResult"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionLoadComment> LoadNewerComments(Guid token, string circleKey, string activityOuterKey, string commentOuterKey, int maxResult = 10)
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionLoadComment>();
            response.Success = false;
            response.Data = new DiscussionLoadComment[0];
            var checkToken = CheckToken(token.ToString());

            if (checkToken == null)
                response.Message = "身分驗證失敗";

            try
            {
                var msgData = new List<DiscussionMessage>();
                var resultData = new DiscussionLoadComment();

                // 是否為合法使用者
                int memberId = checkToken.MemberId;
                if (memberId > 0)
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
                        var commentEventId =Service.Utility.OuterKeyHelper.CheckOuterKey(commentOuterKey);
                        if (commentEventId.HasValue == false)
                        {
                            response.Message = "outerKey格式錯誤";
                            return response;
                        }
                        replyOuterKey = commentEventId.Value;
                        resultData.OuterKey = commentOuterKey;
                        msgData = discussionMsgService.GetMessageList(eventId.Value, replyOuterKey).OrderByDescending(t => t.CreateTime).Take(maxResult).Reverse().ToList();
                        msgData.Remove(msgData.FirstOrDefault(t => t.EventId == replyOuterKey));
                    }
                    //查全部
                    // else
                    // {

                    //  msgData = service.GetMessageList(eventId).OrderBy(t => t.CreateTime).ToList();
                    //resultData.OuterKey = activityOuterKey;
                    // }

                    if (msgData != null)
                    {
                        resultData.Comments = msgData;
                        response.Success = true;
                        response.Message = "查詢成功";
                        response.Data = new DiscussionLoadComment[1] { resultData };

                        // 將訊息發送給群組
                        Clients.Group(circleKey.ToLower()).loadNewerComments(resultData);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "查詢失敗";
                    }
                }
                else
                    Clients.Caller.onError("loadNewerComments", "身分驗證失敗，請重新登入!token:[" + token + "]");
                return response;
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("loadNewerComments", "取得新留言發生意外: " + msg);
                response.Success = false;
                response.Message = "取得新留言發生意外: " + msg;
                return response;
            }
        }


        /// <summary>
        ///  取得舊的資訊
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="activityOuterKey"></param>
        /// <param name="commentOuterKey"></param>
        /// <param name="maxResult"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionLoadComment> LoadOlderComments(Guid token, string circleKey, string activityOuterKey, string commentOuterKey, int maxResult = 10)
        {
            var response = new Infrastructure.ViewModel.Base.ResultBaseModel<DiscussionLoadComment>();
            var checkToken = CheckToken(token.ToString());
            response.Success = false;
            if (checkToken == null)
            {
                response.Message = "身分驗證失敗";
                response.Success = false;
            }
            try
            {
                var msgData = new List<DiscussionMessage>();
                var resultData = new DiscussionLoadComment();
                // 是否為合法使用者
                int memberId = checkToken.MemberId;
                if (memberId > 0)
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
                        var replyEventId = Service.Utility.OuterKeyHelper.CheckOuterKey(commentOuterKey);
                        if (replyEventId.HasValue == false)
                        {
                            response.Message = "outerKey格式錯誤";
                            return response;
                        }
                        replyOuterKey = replyEventId.Value;
                        resultData.OuterKey = commentOuterKey;
                        msgData = discussionMsgService.GetMessageList(eventId.Value, replyOuterKey, false).OrderByDescending(t => t.CreateTime).Take(maxResult).Reverse().ToList();
                    }
                    //查全部
                    // else
                    //  {
                    //      msgData = service.GetMessageList(eventId, null, false).OrderBy(t => t.CreateTime).ToList();
                    //     resultData.OuterKey = activityOuterKey;
                    // }

                    if (msgData != null)
                    {
                        resultData.Comments = msgData;
                        response.Success = true;
                        response.Message = "查詢成功";
                        response.Data = new DiscussionLoadComment[1] { resultData };

                        // 將訊息發送給群組
                        Clients.Group(circleKey.ToLower()).loadOlderComments(resultData);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "查詢失敗";
                    }
                }
                else
                    Clients.Caller.onError("loadOlderComments", "身分驗證失敗，請重新登入!token:[" + token + "]");
                return response;
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("loadNewerComments", "取得新留言發生意外: " + msg);
                response.Success = false;
                response.Message = "取得新留言發生意外: " + msg;
                return response;
            }
        }

        /// <summary>
        /// 按讚
        /// </summary>
        /// <param name="token">使用者代碼</param>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="activityOuterKey">主題討論代碼</param>
        /// <param name="commentOuterKey">留言或回覆代碼 - 如果是留言或回覆按讚</param>
        public async Task<Infrastructure.ViewModel.Base.BaseResponse<string>> SwitchLike(Guid token, string circleKey, string activityOuterKey, string commentOuterKey)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = false;
            var checkToken = CheckTokenToMemberInfo(token);
            if (checkToken == null)
                response.Message = "身分驗證失敗";
            var memberInfo = checkToken;
            if (memberInfo != null)
            {
                var discussionInfo = discussionService.GetDBDiscussionInfo(activityOuterKey);

                var switchResponse = discussionLikeService.SwitchLike(memberInfo, activityOuterKey, commentOuterKey);
                if (switchResponse == null)
                {
                    response.Success = false;
                    response.Message = "outerKey格式錯誤";
                    return response;
                }
                switchResponse.CircleKey = circleKey;
                response.Success = true;
                response.Message = "處理完成";
                Clients.Group(circleKey.ToLower()).updateLikeInfo(switchResponse);
                //如果是對留言或回覆點讚
                if (commentOuterKey != null && commentOuterKey != string.Empty)
                {
                    var msgService = new MessageService();
                    var msgInfo = msgService.GetMsgDBInfoByOuterKey(commentOuterKey);
                    //按讚才要推播
                    if (switchResponse.IsLike)
                    {
                        var eventId = Service.Utility.OuterKeyHelper.CheckOuterKey(commentOuterKey);
                        if (eventId.HasValue == false)
                        {
                            response.Success = false;
                            response.Message = "outerKey格式錯誤";
                            return response;
                        }
                        //是回覆
                        if (msgInfo.Parent != null)
                            await PushLikeAsync(circleKey, eventId.Value, msgInfo.Content, memberInfo, msgInfo.CreateUser.Value, true, true);
                        else
                            await PushLikeAsync(circleKey, eventId.Value, msgInfo.Content, memberInfo, msgInfo.CreateUser.Value, true);
                    }
                }
                //對主題討論點讚
                else
                {
                    //按讚才要推播
                    if (switchResponse.IsLike)
                        await PushLikeAsync(circleKey, discussionInfo.EventId, discussionInfo.Name, memberInfo, discussionInfo.CreateUser.Value, false);
                }
            }
            else
            {
                response.Success = false;
                response.Message = "沒有權限";
            }
            return response;
        }

        #endregion
 
        /// <summary>
        /// 按讚推播
        /// </summary>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="eventId">活動 | 留言代碼</param>
        /// <param name="eventName">按讚資訊</param>
        /// <param name="likeMan">按讚人資訊</param>
        /// <param name="beLikeManId">被按讚人的編號(留言按讚) / 按讚者編號(主題討論按讚)</param>
        /// <param name="isMsg">是否為留言</param>
        /// <param name="isReply">是否為回覆</param>
        /// <returns></returns>
        public async Task PushLikeAsync(string circleKey, Guid eventId, string eventName, Infrastructure.Entity.Member likeMan, int beLikeManId, bool isMsg, bool? isReply = false)
        {
            var memberService = new MemberService();
            //取得長度10以下的內容
            eventName = eventName.Count() > 10 ? eventName.Substring(0, 9) + "..." : eventName;
            var pushMessage = string.Empty;
            var noticeMsg = string.Empty;
            var listAccount = memberService.UserIdToAccount(beLikeManId);
            var pushService = new PushService();
            var memberInfo = likeMan.Id != beLikeManId ? new string[1] { listAccount.Account } : new string[0];
            if (isMsg)
            {
                if (isReply != false)
                {
                    noticeMsg = string.Format("{0}覺得你在主題討論中的回覆很讚：「{1}」", likeMan.Name, eventName);
                    pushMessage = string.Format("{0}覺得你在主題討論中的回覆很讚", likeMan.Name);
                }
                else
                {
                    noticeMsg = string.Format("{0}覺得你在主題討論中的留言很讚：「{1}」", likeMan.Name, eventName);
                    pushMessage = string.Format("{0}覺得你在主題討論中的留言很讚", likeMan.Name);
                }


                if (memberInfo.Count() > 0)
                {
                    //AddNotice(circleKey, eventId, beLikeManId, noticeMsg);
                    noticeService.AddNoticeSaveChange(circleKey, beLikeManId, eventId, noticeMsg);
                    SendNotice(beLikeManId, circleKey);
                    await pushService.PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-留言被按讚", memberInfo, pushMessage);
                }
            }
            else
            {
                pushMessage = string.Format("{0}覺得一則主題討論很讚", likeMan.Name);
                noticeMsg = string.Format("{0}覺得一則主題討論很讚：「{1}」", likeMan.Name, eventName);
                if (memberInfo.Count() > 0)
                {

                    noticeService.AddNoticeSaveChange(circleKey, beLikeManId, eventId, noticeMsg);
                    SendNotice(beLikeManId, circleKey);
                    await pushService.PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-被按讚", memberInfo, pushMessage);
                }
            }
        }

        /// <summary>
        /// 廣播發送訊息
        /// </summary>
        /// <param name="memberId">廣播對象</param>
        /// <param name="circleKey">學習圈代碼</param>
        private void SendNotice(int memberId, string circleKey)
        {
            // signalr發送通知
            var myConn = System.Web.HttpContext.Current.Cache.Get(memberId.ToString()) as List<string>;

            if (myConn != null)
            {
                foreach (var conn in myConn)
                {
                    var data = noticeService.GetNoticeList(circleKey, memberId, 1);
                    Clients.Client(conn).showNoticeList(data);
                    Clients.Client(conn).appendNotice(data);
                }
            }
        }
    }
}
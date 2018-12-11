using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.SignalR;
using WiicoApi.SignalR.MappingConnection;

namespace WiicoApi.SignalRHub
{
    public partial class WiicoHub : WiicoHubBase
    {

        /// <summary>
        /// 發送訊息的動作
        /// </summary>
        /// <param name="circleKey">發送對象(學習圈)</param>
        /// <param name="token">發送者token</param>
        /// <param name="senderName">發送者姓名</param>>
        /// <param name="text">訊息內文</param>
        public IHttpActionResult Message_CreateActivity(Guid token, string circleKey, string senderName, string text)
        {
            var tokenInfo = CheckToken(token.ToString());
            var responseCommonData = new BaseResponse<ActivitysViewModel>();
            responseCommonData.Success = false;
            responseCommonData.Data = new ActivitysViewModel();
            var response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<ActivitysViewModel>>
                                         (responseCommonData, new Controllers.APPSupport.EmptyController());
            if (tokenInfo == null)
                    return response;
            try
            {
                // 是否為合法使用者
                int memberId = tokenInfo.MemberId;
                if (memberId > 0)
                {
                    var learningCircleService = new LearningCircleService();
                    var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey);
                    if (learningCircleInfo!=null)
                    {
                        var messageService = new MessageService();
                        var connections = _connections.GetConnections(circleKey.ToLower());
                        text.Trim().Replace("\r\n", "");
                        if (text != string.Empty || text != null)
                        {
                            var rtn = messageService.Insert(circleKey, learningCircleInfo.Id, memberId, "text", text);
                            responseCommonData.Success = true;
                            responseCommonData.Data = rtn;
                            if (connections != null)
                                foreach (var connectionVersion in connections.Versions)
                                {
                                    //預備傳送的connecitonList列表
                                    var sendConnections = Tools.ConnectionsProcess(connections, connectionVersion);
                                    //原始結果
                                    //if (connectionVersion >= 1)
                                    //大於1版號的使用新版的結果
                                    //rtn = _msgService.Add(circleKey, id, memberId, MessageType.Text, text);

                                    Clients.Clients(sendConnections).appendActivity(rtn, "");
                                }
                            else
                                Clients.Group(circleKey.ToLower()).appendActivity(rtn, "");
                            var signalrService = new SignalrService();
                            //發通知
                            var connectIdAndData = signalrService.GetConnectIdAndData(circleKey.ToLower(), memberId, SignalrConnectIdType.All, SignalrDataType.Activity);
                            SignalrClientHelper.ShowRecordListById(connectIdAndData);

                            // 更新活動紀錄
                            //   ShowRecordList(circleKey.ToLower());

                            // 發送推播通知到行動裝置上(android & ios)
                            PushOnCreatedMessage(circleKey, rtn.OuterKey, rtn.CreatorName, rtn.Text, memberId);
                        }
                    }
                    else {
                        responseCommonData.Message = "Message_CreateActivity 學習圈資訊錯誤，無法發送訊息!";
                        Clients.Caller.onError("Message_CreateActivity", "學習圈資訊錯誤，無法發送訊息!");
                    }
                }
                else
                {
                    Clients.Caller.onError("Message_CreateActivity", "身分驗證失敗，請重新登入!token:[" + token + "]");
                    responseCommonData.Message = "Message_CreateActivity 身分驗證失敗，請重新登入!token:[" + token + "]";
                }
            }
            catch (Exception ex)
            {
                responseCommonData.Message = "Message_CreateActivity 發送訊息發生意外: " + ex.Message;
                Clients.Caller.onError("Message_CreateActivity", "發送訊息發生意外: " + ex.Message);
            }

            response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<ActivitysViewModel>>
                                         (responseCommonData, new Controllers.APPSupport.EmptyController());
            return response;
        }





        #region // 推播
        public async Task PushOnCreatedMessage(string circleKey, Guid eventId, string memberName, string text, int? myId)
        {
            var cacheService = new CacheService();
            var memberService = new MemberService();
            var members = myId.HasValue ? memberService.GetLearningCircleMembers(circleKey.ToLower(), myId) : memberService.GetLearningCircleMembers(circleKey.ToLower());
            //var members = cacheService.GetCircleMember(circleKey.ToLower()).Select(t=>t.Account);
            // new CacheCircle().GetCircleMemberList(circleKey, myId);
            // 推播文字→ {建立者姓名} :{留言內容}
            var message = string.Format("{0} :{1}", memberName, text);
            var pushService = new PushService();
            if (members.Count() > 0)
                await pushService.PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看新的-互動文字", members.Select(t => t.Account).ToArray(), message);
        }
        #endregion
    }
}
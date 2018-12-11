using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.FirebasePush;
using WiicoApi.Repository;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;

namespace WiicoApi.Service.CommenService
{
    public class PushService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly MemberService memberService = new MemberService();
        private readonly CacheService cacheService = new CacheService();
        private readonly LearningCircleService learningCircleService = new LearningCircleService();
        private readonly DiscussionFuncMsg discussionMsgService = new DiscussionFuncMsg();
        private readonly NoticeService noticeService = new NoticeService();

        public PushService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 點名堆播資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public async Task PushOnCreatedSignIn(string circleKey, Guid eventId, int? myId)
        {
            var creatorName = string.Empty;

            if (myId != null)
            {
                creatorName = memberService.UserIdToAccount(myId.Value).Name;
            }

            var members = learningCircleService.GetCircleMemberList(circleKey, myId);
            var eventMessage = string.Format("{0}新增了點名活動", creatorName);

            // 推播文字:點名活動即將開始({日期} {時間})
            // var message = string.Format("{0}(建立時間:{1:yyyy/M/d HH:mm})", eventMessage, DateTime.Now);
            var message = string.Format("{0}", eventMessage);
            if (members.Count > 0)
                await PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看點名-開始", members.ToArray(), message);
        }

        /// <summary>
        /// 新增公版活動推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="typeName"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public async Task PushPublishOnCreatedAsync(string circleKey, Guid eventId, string eventName, string typeName, int? myId)
        {
            var creatorName = string.Empty;

            if (myId != null)
            {
                creatorName = memberService.UserIdToAccount(myId.Value).Name;
            }
            var members = learningCircleService.GetCircleMemberList(circleKey, myId);
            var eventMessage = string.Format("{1} 新增了{2}：{0}", eventName, creatorName, typeName);
            if (members.ToList().Count > 0)
                await PushMsgAsync("ToEventCard", circleKey, eventId, string.Format("推播_查看新的-公版活動:{0}", typeName), members.ToArray(), eventMessage);
        }

        /// <summary>
        /// 學生繳交作業推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="studentId"></param>
        /// <param name="updateStatus"></param>
        /// <returns></returns>
        public async Task PushStudentUploadHomeWorkAsync(string circleKey, Guid eventId, string[] pushMember, int Filecount)
        {
            var _student = string.Empty;
            if (pushMember.Any())
            {
                _student = pushMember[1];
            }

            var eventMessage = string.Format("{0}繳交作業，上傳{1}個檔案", _student, Filecount);

            await PushMsgAsync("ToEventCard", circleKey, eventId, "上傳作業", pushMember, eventMessage);
        }

        /// <summary>
        /// 新增作業活動推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task PushHomeWorkMessageOnCreatedAsync(string circleKey, Guid eventId, string eventName, int? myId)
        {
            var members = learningCircleService.GetCircleMemberList(circleKey, myId);

            var eventMessage = string.Format("老師新增了個人作業：{0}", eventName);
            if (members.Count > 0)
                await PushMsgAsync("ToEventCard", circleKey, eventId, "新增作業活動", members.ToArray(), eventMessage);
        }

        /// <summary>
        /// 新增主題討論活動推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public async Task PushDiscussionOnCreatedAsync(string circleKey, Guid eventId, string eventName, int? myId)
        {
            var creatorName = string.Empty;

            if (myId != null)
                creatorName = memberService.UserIdToAccount(myId.Value).Name;

            var members = learningCircleService.GetCircleMemberList(circleKey, myId);
            var eventMessage = string.Format("{1}新增了主題討論：{0}", eventName, creatorName);
            if (members.Count > 0)
                await PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看新的-主題討論", members.ToArray(), eventMessage);
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
        public async Task PushDiscussionOnCreatedAsync(List<Infrastructure.BusinessObject.MemberCacheData> members, string circleKey, Guid eventId, string eventName, int myId)
        {
            //刪除自己
            members.Remove(members.FirstOrDefault(t => t.Id == myId));
            var creatorName = memberService.UserIdToAccount(myId).Name;
            var eventMessage = string.Format("{1}新增了主題討論：{0}", eventName, creatorName);
            if (members.Count() > 0)
                await PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看新的-主題討論", members.Select(t => t.Account).ToArray(), eventMessage);
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
        public async Task PushDiscussionOnUpdateAsync(List<Infrastructure.BusinessObject.MemberCacheData> members, string circleKey, Guid eventId, string eventName, int myId)
        {
            //刪除自己
            members.Remove(members.FirstOrDefault(t => t.Id == myId));
            var creatorName = memberService.UserIdToAccount(myId).Name;
            var eventMessage = string.Format("[{0}]內容已更新", eventName);
            var noticeMsg = string.Format("{0} 更新了主題討論 : 「{1} 」", creatorName, eventName);
            //新增多筆訊息資料
            noticeService.AddMultipleNotice(members, eventId, circleKey, noticeMsg);

            if (members.Count() > 0)
                //推播
                await PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看更新的主題討論", members.Select(t => t.Account).ToArray(), eventMessage);
        }

        /// <summary>
        /// 主題討論留言推播
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="replyMemberInfo"></param>
        /// <returns></returns>
        public async Task PushDiscussionSendMsgAsync(string circleKey, Guid eventId, string eventName, Infrastructure.Entity.Member replyMemberInfo)
        {
            var eventMessage = string.Format("{0}回應了一則主題討論", replyMemberInfo.Name);
            var noticeMsg = string.Format("{0}回應了一則主題討論：「 {1}」", replyMemberInfo.Name, eventName);
            var replyMembers = discussionMsgService.GetReplyMemberList(eventId, replyMemberInfo).ToList();
            //新增多筆訊息資料
            noticeService.AddMultipleNotice(replyMembers, eventId, circleKey, noticeMsg);

            var pushMembers = replyMembers.Select(t => t.Account).ToArray();
            if (replyMembers.Count > 0)
                await PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-被留言", pushMembers, eventMessage);
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
        public async Task PushMsgReplyAsync(string circleKey, Guid eventId, string eventName, Infrastructure.Entity.Member replyMember, Infrastructure.Entity.Member beReplyMemberInfo, bool? isTag = false)
        {
            var eventMessage = isTag.Value ? (string.Format("{0}回應了一則主題討論的回覆", replyMember.Name)) : (string.Format("{0}回應了一則主題討論的留言", replyMember.Name));
            var noticeMsg = isTag.Value ? string.Format("{0}回應了一則主題討論的回覆：「{1}」", replyMember.Name, eventName) : string.Format("{0}回應了一則主題討論的留言：「{1}」", replyMember.Name, eventName);
            // var pushMember =replyMember.Id!= beReplyMemberInfo.Id ? new string[1] { beReplyMemberInfo.Account } : new string[0];
            var replyMembers = discussionMsgService.GetReplyMemberList(eventId, replyMember).ToList();
            if (replyMembers.Count() > 0)
            {
                //新增多筆訊息資料
                noticeService.AddMultipleNotice(replyMembers, eventId, circleKey, noticeMsg);

                if (isTag.Value)
                    await PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-回覆被回覆", replyMembers.Select(t => t.Account).ToArray(), eventMessage);
                else
                    await PushMsgAsync("ToDiscussionActivity", circleKey, eventId, "推播_查看主題討論-留言被回覆", replyMembers.Select(t => t.Account).ToArray(), eventMessage);
            }
        }

        /// <summary>
        /// 推播請假單資訊給老師們
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="myId"></param>
        /// <param name="leaveDate"></param>
        /// <returns></returns>
        public async Task<string> PushOnCreatedLeave(string circleKey, Guid eventId, int? myId, DateTime leaveDate)
        {
            var members = learningCircleService.GetCircleTeacherListBySql(circleKey, myId);
            //var eventMessage = "請假單申請";
            var stuName = string.Empty;
            if (myId != null)
            {
                stuName = memberService.UserIdToAccount(Convert.ToInt32(myId)).Name;
            }
            // 推播文字:點名活動即將開始({日期} {時間})
            var pushMsg = string.Format("{0}新增了一張({1:yyyy/MM/dd})的請假單", stuName, leaveDate);
            var noticeMsg = string.Format("{0}新增了一張({1:yyyy/MM/dd})的請假單", stuName, leaveDate);

            if (members.Count > 0)
                PushMsgAsync("ToLeave", circleKey, eventId, "推播_請假-收到假單", members.ToArray(), pushMsg);

            return noticeMsg;
        }

        /// <summary>
        /// 推播審核結果給學生
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="myId"></param>
        /// <param name="pushAccount"></param>
        /// <param name="leaveDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<string> PushOnUpdatedLeave(string circleKey, Guid eventId, int? myId, DateTime leaveDate, string status)
        {
            string pushAccount = memberService.UserIdToAccount(Convert.ToInt32(myId)).Account;

            var members = new string[1] {
                pushAccount
            };
            //var eventMessage = "審核請假單";
            var circle = cacheService.GetCircle(circleKey);
            // 推播文字:點名活動即將開始({日期} {時間})
            //var message = string.Format("您在(請假時間:{0:yyyy/MM/dd})的請假單已經({1})", leavseDate, status);
            var message = string.Format("您在({0:yyyy/MM/dd})的請假單「{1}」", leaveDate, status);
            var gaText = string.Format("推播_請假_看結果-{0}", status);
            var noticeTxt = string.Empty;
            //組通知訊息
            // var noticeMsg = string.Format("您在(請假時間:{0:yyyy/MM/dd})的請假單已經({1}) [{2}] (批單時間:{3:yyyy/MM/dd})", leaveDate, status, circle.Name, DateTime.Now);
            var noticeMsg = string.Format("您在({0:yyyy/MM/dd})的請假單「{1}」", leaveDate, status, circle.Name, DateTime.Now);

            if (members.Count() > 0)
                await PushMsgAsync("ToLeave", circleKey, eventId, gaText, members.ToArray(), message);

            return noticeMsg;
        }

        /// <summary>
        /// 老師新增教材
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public async Task PushMaterialOnCreatedAsync(string circleKey, Guid eventId, string eventName, int? myId)
        {
            var members = learningCircleService.GetCircleMemberList(circleKey, myId);

            var memberInfo = memberService.UserIdToAccount(Convert.ToInt32(myId));
            var eventMessage = string.Format("{0}傳送了圖片", memberInfo.Name, eventName);

            await PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看新的-互動圖片", members.ToArray(), eventMessage);

        }

        /// <summary>
        /// 可輸入systemId判斷字串，功能與舊的一樣
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="pushType"></param>
        /// <param name="students"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PushMsgAsync(string systemId, string circleKey, Guid eventId, string pushType, string[] students, string message)
        {
            var learningCircleInfo = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == circleKey.ToLower());
            message = string.Format("[{0}] {1}", learningCircleInfo.Name, message);
            //firebase推播
            var firebasePushService = new FirebasePushService();
            var eventOuterKey = Utility.OuterKeyHelper.GuidToPageToken(eventId);
            var jsonString = "{" + string.Format("\"title\":\"{0}\",\"msg\":\"{1}\",\"systemId\":\"{2}\",\"gaEvent\":\"{3}\",\"circleKey\":\"{4}\",\"eventOuterkey\":\"{5}\"", message, message, systemId, pushType, circleKey, eventOuterKey) + "}";
            var pushDataInfo = firebasePushService.CreatePushData(message, message, systemId, pushType, circleKey, eventOuterKey);
            var newPushResponse = firebasePushService.SendMutiplePushNotification(jsonString, circleKey, students.ToList(), pushDataInfo.Id, message);

            // 載入推播設定
            //    var config = LoadConfig();
            // 依照設定建立推播服務
            //  var service = new ServiceAdapter(config);

            // 產生推播資料 - 1.APP判斷用字串
            //string systemId = "ToEventCard";

            //// 產生推播資料 - 2.推播文字: [{課名}] [模組文字]
            //var circle = cacheService.GetCircle(circleKey);
            //string text = string.Format("[{0}] {1}", circle.Name, message);

            //// 產生推播資料 - 3.APP使用的參數
            //var info = new iThink.Infrastructure.BusinessObject.CourseActivityInfo();
            //info.CircleKey = circleKey;
            //info.EventOuterKey = eventOuterKey;
            //info.GaEvent = pushType;
            ///* var info = new string[3];
            // info[0] = circleKey;
            // info[1] = Utility.GuidToPageToken(eventId);
            // info[2] = pushType;*/

            //// 實際推播
            ////var pushCommand = new PushCommand();
            ////ed8fee18-e3a9-4fbe-ac79-4a316a07e043
            ////var token = "2F1A0D15-8659-4469-A5E1-092E15F9127F";

            ////查目前的AccessToken
            //var db = _uow.DbContext;
            //var pushAccess = db.PushAccessToken.FirstOrDefault();
            //var _now = DateTime.UtcNow;
            //var token = string.Empty;
            //if (pushAccess != null)
            //{
            //    //判斷是否過期
            //    if (pushAccess.Created.Utc.Value.Hour != _now.Hour)
            //    {
            //        token = await service.RefreshAccessTokenAsync();
            //        pushAccess.token = token;
            //        pushAccess.Created = Infrastructure.Property.TimeData.Create(_now);
            //        db.SaveChanges();
            //    }
            //    else
            //    {
            //        token = pushAccess.token;
            //    }
            //}
            //else
            //{
            //    token = await service.RefreshAccessTokenAsync();
            //    pushAccess = new Infrastructure.Entity.PushAccessToken();
            //    pushAccess.token = token;
            //    pushAccess.Created = Infrastructure.Property.TimeData.Create(_now);
            //    db.PushAccessToken.Add(pushAccess);
            //    db.SaveChanges();
            //}

            // var token = await service.RefreshAccessTokenAsync();

            //  var parameters = CreatePushClassParametersForActivity(token, systemId, info, students.ToArray(), text);
            //    var result = await service.SendPushRequestAsync(parameters);
        }
    }
}

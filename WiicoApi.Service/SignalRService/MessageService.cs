using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.Service.SignalRService
{
    /// <summary>
    /// 活動留言服務
    /// </summary>
    public class MessageService
    {
        private readonly GenericUnitOfWork _uow;

        public MessageService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 發起一個訊息活動
        /// </summary>
        /// <param name="learningId"></param>
        /// <param name="memberId"></param>
        /// <param name="key"></param>
        /// <param name="duration"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActivitysViewModel Insert(string circleKey, int circleId, int memberId, string type, string text)
        {
            using (var db = _uow.DbContext)
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dt = DateTime.UtcNow;
                        var eventId = Guid.NewGuid();

                        var actMessageEntity = new Infrastructure.Entity.ActMessage()
                        {
                            Content = text,
                            Created = TimeData.Create(dt),
                            Updated = TimeData.Create(null),
                            Deleted = TimeData.Create(null),
                            CreateUser = memberId,
                            EventId = eventId,
                            LearningId = circleId,
                            Type = "text",
                            Visibility = true
                        };
                        var activityEntity = new Infrastructure.Entity.Activitys()
                        {
                            CardisShow = true,
                            Created = TimeData.Create(dt),
                            Updated = TimeData.Create(null),
                            Deleted = TimeData.Create(null),
                            CreateUser = memberId,
                            IsActivity = true,
                            ModuleKey = ModuleType.Message,
                            Publish_Utc = dt,
                            ToRoomId = circleKey,
                            StartDate = dt,
                            OuterKey = eventId
                        };

                        db.ActMessage.Add(actMessageEntity);
                        db.Activitys.Add(activityEntity);
                        var memberService = new MemberService();
                        var memberInfo = memberService.UserIdToAccount(memberId);
                        var sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(eventId);
                        var data = new ActivitysViewModel()
                        {
                            Publish_Utc = dt,
                            Created_Utc = dt,
                            CreatorAccount = memberInfo.Account,
                            CreatorName = memberInfo.Name,
                            CreatorPhoto = memberInfo.Photo,
                            ModuleKey = ModuleType.Message,
                            OuterKey = eventId,
                            Text = text,
                            ToRoomId = circleKey,
                            sOuterKey = sOuterKey
                        };
                        db.SaveChanges();
                        dbTransaction.Commit();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        dbTransaction.Rollback();
                        return null;
                    }
                }
            }
        }

        public MessageViewModel Get(Guid eventId)
        {
            var msg = _uow.DbContext.ActMessage.FirstOrDefault(x => x.EventId == eventId);
            var activityInfo = _uow.ActivitysRepo.GetFirst(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return null;
            var vm = new MessageViewModel()
            {
                strEventId = Utility.OuterKeyHelper.GuidToPageToken(eventId),
                Type = msg.Type,
                Content = msg.Content,
                Publish_Utc = activityInfo.Publish_Utc
            };
            return vm;
        }

        /// <summary>
        /// 取得活動留言資訊 - 根據加密過的留言代碼字串取得
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public ActModuleMessage GetMsgDBInfoByOuterKey(string outerKey)
        {
            var msgEventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (msgEventId.HasValue)
                return GetMsgDBInfoByEventId(msgEventId.Value);
            else
                return null;
        }

        /// <summary>
        /// 取得活動留言資訊 - 根據實際的留言代碼取得
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public ActModuleMessage GetMsgDBInfoByEventId(Guid eventId)
        {
            var db = _uow.DbContext;
            var msgInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == eventId);
            return msgInfo != null ? msgInfo : null;
        }
    }
}

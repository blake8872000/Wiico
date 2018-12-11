using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Repository;

namespace WiicoApi.Service.SignalRService
{
    /// <summary>
    /// 通知服務
    /// </summary>
    public class NoticeService
    {
        private readonly GenericUnitOfWork _uow;

        public NoticeService()
        {
            _uow = new GenericUnitOfWork();
        }


        /// <summary>
        /// 發送鈴鐺訊息
        /// </summary>
        /// <param name="members"></param>
        /// <param name="eventId"></param>
        /// <param name="circleKey"></param>
        /// <param name="noticeMsg"></param>
        /// <returns></returns>
        public void AddMultipleNotice(List<Infrastructure.BusinessObject.MemberCacheData> members, Guid eventId, string circleKey, string noticeMsg)
        {
            var db = _uow.DbContext;
            foreach (var member in members)
            {
                //發訊息
                AddNotice(circleKey, member.Id, eventId, noticeMsg);
            }
            db.SaveChanges();
        }
        public void AddNoticeSaveChange(string circleKey, int memberId, Guid eventId, string text)
        {
            var db = _uow.DbContext;
            AddNotice(circleKey, memberId, eventId, text);
            db.SaveChanges();
        }

        public void AddNotice(string circleKey, int memberId, Guid eventId, string text)
        {
            var db = _uow.DbContext;
            var data = new ActivitysNotices()
            {
                ToRoomId = circleKey,
                MemberId = memberId,
                EventId = eventId,
                NoticeContent = text,
                HasClick = false,
                CreateTime = DateTime.UtcNow
            };
            db.ActivitysNotice.Add(data);


            //       return GetNoticeList(circleKey,memberId, 1);
        }

        public ActivitysNoticeViewModel GetNoticeList(string circleKey, int memberId, int maxResult, int? ActivityNoticeId = 0)
        {
            var db = _uow.DbContext;
            var result = new ActivitysNoticeViewModel();
            //查出目前MemberId在circleKey中的已讀情況
            var checkLastRead = db.ActivitysReadMark.FirstOrDefault(t => t.ToRoomId == "myNotice" && t.memberId == memberId);
            var unReadCount = (checkLastRead != null && checkLastRead.LastReadActivityIdEnd > 0) ?
                db.ActivitysNotice.Where(t => t.ToRoomId.ToLower() == circleKey.ToLower() && t.Id > checkLastRead.LastReadActivityIdEnd && t.MemberId == memberId).Count() :
                db.ActivitysNotice.Where(t => t.ToRoomId.ToLower() == circleKey.ToLower() && t.MemberId == memberId).Count();
            var finalData = new List<Infrastructure.ValueObject.ActivitysNoticeData>();
            //只取得活動相關的動態通知
            var actData = (from n in db.ActivitysNotice
                           join lc in db.LearningCircle on n.ToRoomId equals lc.LearningOuterKey
                           join a in db.Activitys on n.EventId equals a.OuterKey
                           join m in db.Members on n.MemberId equals m.Id
                           where n.MemberId == memberId
                           select new Infrastructure.ValueObject.ActivitysNoticeData()
                           {
                               CircleId = lc.Id,
                               CircleName = lc.Name,
                               Id = n.Id,
                               EventId = n.EventId,
                               NoticeContent = n.NoticeContent,
                               HasClick = n.HasClick,
                               CreateTime = n.CreateTime,
                               ModuleKey = a.ModuleKey,
                               Title = (a.ModuleKey == "signIn" && a.ModuleKey != null) ? "點名活動(" + a.Created.Utc.Value.Year + "/" + a.Created.Utc.Value.Month + "/" + a.Created.Utc.Value.Day + " " + a.Created.Utc.Value.Hour + ":" + a.Created.Utc.Value.Minute + ")" : null,
                               ToRoomId = lc.LearningOuterKey,
                               DeleteTime = a.Deleted.Utc,
                               //  IsDelete = a.Deleted.Utc.HasValue ? true : false,
                               UnreadCount = unReadCount                             
                           }).OrderByDescending(t => t.CreateTime).Take(maxResult);
            //只取得訊息相關的動態通知
            var msgData = (from n in db.ActivitysNotice
                           join lc in db.LearningCircle on n.ToRoomId equals lc.LearningOuterKey
                           join amm in db.ActModuleMessage on n.EventId equals amm.OuterKey
                           join a in db.Activitys on amm.ActivityId equals a.Id
                           join m in db.Members on n.MemberId equals m.Id
                           where n.MemberId == memberId
                           select new Infrastructure.ValueObject.ActivitysNoticeData()
                           {
                               CircleId = lc.Id,
                               CircleName = lc.Name,
                               Id = n.Id,
                               EventId = n.EventId,
                               NoticeContent = n.NoticeContent,
                               HasClick = n.HasClick,
                               CreateTime = n.CreateTime,
                               ModuleKey = a.ModuleKey,
                               ToRoomId = lc.LearningOuterKey,
                               DeleteTime = a.Deleted.Utc,
                               // IsDelete = a.Deleted.Utc.HasValue ? true : false,
                               UnreadCount = unReadCount
                           }).OrderByDescending(t => t.CreateTime).Take(maxResult);


            if (ActivityNoticeId > 0)
            {
                actData = actData.Where(t => t.Id < ActivityNoticeId);
                msgData = msgData.Where(t => t.Id < ActivityNoticeId);
            }

            
            finalData.AddRange(actData);
            finalData.AddRange(msgData);
            foreach (var data in finalData)
            {
                data.IsDelete = (data.DeleteTime.HasValue ? true : false);
                data.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(data.EventId);
            }
            result.UnreadCount = unReadCount;
            result.Data = finalData.OrderByDescending(t => t.CreateTime).Take(maxResult).ToList();
            return result;
        }
    }
}

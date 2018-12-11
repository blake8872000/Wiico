using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.Service.SignalRService
{
    public class ActivityService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly MemberService memberService = new MemberService();

        public ActivityService()
        {
            _uow = new GenericUnitOfWork();
        }

        public ActivityService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }

        public ActivityService(string connectionName)
        {
            _uow = new GenericUnitOfWork(connectionName);
        }

        /// <summary>
        /// 取得某個活動模組最新的活動
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public Activitys GetNewestByModuleType(string moduleType,string circleKey) {
            return _uow.DbContext.Activitys.Where(t => t.ToRoomId.ToLower() == circleKey.ToLower() && t.ModuleKey == ModuleType.SignIn).OrderByDescending(t => t.Publish_Utc).FirstOrDefault();
        }

        /// <summary>
        /// 判斷是否有某個活動模組正在進行中
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public bool CheckActivityStarting(string circleKey , string moduleType) {
            var db = _uow.DbContext;
            return (from acts in db.Activitys
                        where acts.ModuleKey == ModuleType.SignIn
                        && acts.ToRoomId.ToLower() == circleKey.ToLower()
                        && DbFunctions.DiffSeconds(acts.StartDate, DateTime.UtcNow) < acts.Duration

                        select "1").Any();
        }

        #region // 通知
        public bool AddMutipleNotice(string circleKey, List<int> memberIds, Guid eventId, string text)
        {
            var noticeRep = _uow.EntityRepository<ActivitysNotices>();
            foreach (var memberId in memberIds)
            {
                var data = new ActivitysNotices()
                {
                    ToRoomId = circleKey,
                    MemberId = memberId,
                    EventId = eventId,
                    NoticeContent = text,
                    HasClick = false,
                    CreateTime = DateTime.UtcNow
                };
                noticeRep.Insert(data);
            }
            // 2018-04-02 yuschang 這裡透過捕獲異常的方式將例外重新封裝並向上拋出
            try
            {
                _uow.SaveChanges();

                return true;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                return false;
                throw raise;
            }
        }

        public bool AddNotice(string circleKey, int memberId, Guid eventId, string text)
        {
            var noticeRep = _uow.EntityRepository<ActivitysNotices>();
            var data = new ActivitysNotices()
            {
                ToRoomId = circleKey,
                MemberId = memberId,
                EventId = eventId,
                NoticeContent = text,
                HasClick = false,
                CreateTime = DateTime.UtcNow
            };
            noticeRep.Insert(data);

            // 2018-04-02 yuschang 這裡透過捕獲異常的方式將例外重新封裝並向上拋出
            try
            {
                _uow.SaveChanges();

                return true;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                return false;
                throw raise;
            }
        }

        public ActivitysNoticeViewModel GetNoticeList(int memberId, int maxResult, int? ActivityNoticeId = 0)
        {

            var data = _uow.ActivitysNoticesRepo.GetActivitysNoticeDatas(memberId, maxResult, ActivityNoticeId);

            /* foreach(var item in data)
             {
                 item.OuterKey = DeveloperTools.ApiData.Utility.GuidToPageToken(item.EventId);
             }*/

            var rtn = new ActivitysNoticeViewModel()
            {
                UnreadCount = (data.FirstOrDefault() == null ? 0 : data.FirstOrDefault().UnreadCount),
                Data = data
            };

            return rtn;
        }

        public void ClearNoticeCount(int memberId)
        {
            //_uow.ActivitysReadMarksRepo.SaveActivitysReadMarks(memberId);
            var db = _uow.DbContext;

            var noticeInfo = db.ActivitysNotice.Where(t => t.MemberId == memberId).OrderByDescending(t => t.Id).FirstOrDefault();
            if (noticeInfo != null)
            {
                var readMarkInfo = db.ActivitysReadMark.FirstOrDefault(t => t.memberId == memberId && t.ToRoomId == "myNotice");
                if (readMarkInfo == null)
                {
                    var entity = new Infrastructure.Entity.ActivitysReadMark()
                    {
                        Enabled = true,
                        memberId = memberId,
                        LastReadActivityIdBegin = 1,
                        LastReadActivityIdEnd = noticeInfo.Id,
                        Time = DateTime.UtcNow,
                        ToRoomId = "myNotice"
                    };
                    db.ActivitysReadMark.Add(entity);
                }
                else
                {
                    readMarkInfo.LastReadActivityIdEnd = noticeInfo.Id;
                    readMarkInfo.Time = DateTime.UtcNow;
                }
                db.SaveChanges();
            }
        }

        public void UpdateNoticeClick(int memberId, int id)
        {
            var Rep = _uow.EntityRepository<ActivitysNotices>();
            var notice = Rep.GetFirst(x => x.Id == id);
            if (notice.MemberId == memberId)
            {
                notice.HasClick = true;
                _uow.SaveChanges();
            }
        }

        #endregion

        /// <summary>
        /// 取得某個課程的某種活動列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="moduleKey"></param>
        /// <returns></returns>
        public IEnumerable<Activitys> GetActivityListByModuleKey(string circleKey, string moduleKey, int? pages = 1, int? rows = 20)
        {
            return (rows.HasValue && pages.HasValue) ?
          _uow.EntityRepository<Activitys>().Get(t => t.ModuleKey == moduleKey && t.ToRoomId == circleKey && t.Deleted.Utc == null && t.CardisShow == true).OrderByDescending(t => t.Publish_Utc).Skip((pages.Value - 1) * rows.Value).Take(rows.Value) :
          _uow.EntityRepository<Activitys>().Get(t => t.ModuleKey == moduleKey && t.ToRoomId == circleKey && t.Deleted.Utc == null && t.CardisShow == true).OrderByDescending(t => t.Publish_Utc);
        }

        /// <summary>
        /// 取得單一活動資訊
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Activitys GetByEventId(Guid eventId)
        {
            return _uow.DbContext.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public bool DeleteLatest(int memberId, string circleKey)
        {
            var activitysReadMarkInfo = _uow.EntityRepository<ActivitysReadMark>().GetFirst(t => t.memberId == memberId && t.ToRoomId == circleKey);
            if (activitysReadMarkInfo != null)
            {
                activitysReadMarkInfo.Enabled = false;
                activitysReadMarkInfo.UpdateDate_Utc = DateTime.UtcNow;
                _uow.SaveChanges();
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// 取得課第一筆活動資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="createtime"></param>
        /// <returns></returns>
        public Activitys GetFirstActivity(string circleKey)
        {
            var result = _uow.DbContext.Activitys.Where(t => t.ToRoomId.ToLower() == circleKey.ToLower() && t.CardisShow == true && t.IsActivity == true).OrderBy(t => t.Publish_Utc).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 取得課最後一筆已讀活動資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="createtime"></param>
        /// <returns></returns>
        public Activitys GetLastActivity(string circleKey)
        {
            var result = _uow.DbContext.Activitys.Where(t => t.ToRoomId == circleKey && t.CardisShow == true && t.IsActivity == true).OrderByDescending(t => t.Publish_Utc).FirstOrDefault();
            return result;
        }

        public IEnumerable<ActivitysLatest> GetLatestList(int memberId, string circleKey)
        {
            // var result = _uow.ActivitysRepo.GetActivitysLatest(memberId, circleKey);
            var result = _uow.ActivitysRepo.GetActivitysLatestByLinQ(memberId, circleKey);
            var db = _uow.DbContext;
            if (result == null)
                return null;
            foreach (var item in result)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(item.MemberName);

                if (item.ActType == QueryCondition.ModuleType.Message)
                {
                    //有姓名的話要加冒號
                    if (!string.IsNullOrEmpty(item.MemberName))
                        sb.Append("：");
                    var msgInfo = db.ActMessage.FirstOrDefault(t => t.EventId == item.EventId);
                    if (msgInfo != null)
                        sb.Append(msgInfo.Content);
                }
                else if (item.ActType == QueryCondition.ModuleType.Material)
                    sb.Append("傳送了圖片");
                else if (item.ActType == QueryCondition.ModuleType.Discussion)
                {
                    sb.Append("新增了");
                    sb.Append(QueryCondition.ModuleType.GetModuleName(item.ActType));
                    sb.Append(":「");
                    var discussionInfo = db.ActDiscussion.FirstOrDefault(t => t.EventId == item.EventId);
                    if (discussionInfo != null)
                        sb.Append(discussionInfo.Name);
                    sb.Append("」");
                }
                else if (item.ActType == QueryCondition.ModuleType.Group)
                {
                    sb.Append("新增了");
                    sb.Append(QueryCondition.ModuleType.GetModuleName(item.ActType));
                    sb.Append(":「");
                    var groupInfo = db.ActGroupCategory.FirstOrDefault(t => t.EventId == item.EventId);
                    if (groupInfo != null)
                        sb.Append(groupInfo.Name);
                    sb.Append("」");
                }
                else if (item.ActType == QueryCondition.ModuleType.General)
                {
                    sb.Append("新增了");

                    var generalInfo = db.ActGeneral.FirstOrDefault(t => t.EventId == item.EventId);
                    if (generalInfo != null)
                        sb.Append(generalInfo.Content);
                }
                else if (item.ActType == QueryCondition.ModuleType.Vote)
                {
                    sb.Append("新增了");
                    sb.Append(QueryCondition.ModuleType.GetModuleName(item.ActType));
                    sb.Append(":「");
                    var voteInfo = _uow.ActVoteRepo.GetFirst(t => t.EventId == item.EventId);
                    if (voteInfo != null)
                        sb.Append(voteInfo.Content);

                    sb.Append("」");
                }
                else
                {
                    sb.Append("新增了");
                    sb.Append(QueryCondition.ModuleType.GetModuleName(item.ActType));
                }
                item.Text = sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// 顯示一張活動卡片
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="type"></param>
        /// <param name="memberId"></param>
        /// <param name="isShowCard"></param>
        /// <returns></returns>
        public ActivitysViewModel SignalrResponse(string circleKey, Guid eventId, string type, int memberId, bool isShowCard)
        {
            var db = _uow.DbContext;
            var activityInfo = db.Activitys.Where(t => t.OuterKey == eventId && t.ToRoomId == circleKey).FirstOrDefault();
            if (activityInfo != null)
            {
                try
                {
                    var me = memberService.UserIdToAccount(memberId);
                    activityInfo.CardisShow = true;
                    //更新狀態為顯示卡片
                    db.SaveChanges();
                    DateTime dt = DateTime.UtcNow;
                    var rtn = new ActivitysViewModel()
                    {
                        Id = activityInfo.Id,
                        sOuterKey =Utility.OuterKeyHelper.GuidToPageToken(eventId),
                        CreatorAccount = me.Account,
                        CreatorName = me.Name,
                        CreatorPhoto = me.Photo,
                        ModuleKey = type,
                        Created_Utc = dt,
                        ReadMark = false,
                        PositionMark = false,
                        OuterKey = eventId,
                        ToRoomId = circleKey.ToLower(),
                        Publish_Utc = activityInfo.Publish_Utc
                    };
                    return rtn;
                }
                catch (Exception ex)
                {
                    return null;
                    throw ex;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// 在水道頁長出新增活動之後的上傳作業活動
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ActivitysViewModel addPanelInfo(Guid eventId, int memberId)
        {
            var data = new ActivitysViewModel();
            var db = _uow.DbContext;

            DateTime dt = DateTime.UtcNow;
            // 回傳給APP的物件
            var me = db.Members.SingleOrDefault(x => x.Id == memberId);
            if (me != null)
            {
                data = new ActivitysViewModel()
                {
                    Id = 0,
                    CreatorAccount = me.Account,
                    CreatorName = me.Name,
                    ModuleKey = "upload",
                    Created_Utc = dt,
                    ReadMark = false,
                    PositionMark = false,
                    OuterKey = eventId

                };
            }

            return data;
        }

        ///// <summary>
        ///// 用於查詢日期
        ///// </summary>
        ///// <param name="circleKey"></param>
        ///// <param name="memberId"></param>
        ///// <param name="maxResult"></param>
        ///// <param name="pageToken"></param>
        ///// <param name="queryDate"></param>
        ///// <returns></returns>
        public ReadMarkResult<ActivitysViewModel> GetQueryDateList(string circleKey, int memberId, int maxResult, Guid pageToken, DateTime? queryDate = null)
        {
            var result = new ReadMarkResult<ActivitysViewModel>();
            var list = new List<ActivitysViewModel>();

            try
            {
                // 取出最新的 maxResult 筆資料
                list = _uow.ActivitysRepo.GetQueryDateList(circleKey, memberId, maxResult, pageToken).ToList();

                foreach (var item in list)
                {
                    item.sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(item.OuterKey);
                }

                // 更新接口資訊
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                result.Data = default(ActivitysViewModel[]);
            }

            bool goback = false; //是否往前取得活動

            var param = new SortParam()
            {
                MaxResult = maxResult,
                MemberId = memberId,
                CircleKey = circleKey,
                Goback = goback
            };
            if (queryDate != null)
                param.QueryDateTime = queryDate.Value;

            return SortList(result, list, param);
        }

        /// <summary>
        /// 取得已讀或未讀數量
        /// </summary>
        /// <param name="isRead"></param>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public int GetActivityReadCount(bool isRead, string outerKey)
        {
            var db = _uow.DbContext;
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);

            var activityInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return 0;
            var count = isRead ?
                //已讀
                (from rm in db.ActivitysReadMark
                 join a in db.Activitys on rm.ToRoomId equals a.ToRoomId
                 where a.ToRoomId == activityInfo.ToRoomId &&
                 (rm.LastReadActivityIdBegin >= activityInfo.Id && rm.LastReadActivityIdEnd <= activityInfo.Id)
                 select rm).Count() :
                 //未讀
                 (from rm in db.ActivitysReadMark
                  join a in db.Activitys on rm.ToRoomId equals a.ToRoomId
                  where a.ToRoomId == activityInfo.ToRoomId &&
                  rm.LastReadActivityIdEnd >= activityInfo.Id
                  select rm).Count();
            return count;
        }
        /// <summary>
        /// 取得使用者在課堂上最後已讀活動資料
        /// </summary>
        /// <param name="isRead"></param>
        /// <param name="outerKey"></param>
        /// 
        /// <returns></returns>
        public Guid GetMemberLastReadActivity(string circleKey, int memberId)
        {
            var result = Guid.Empty;
            var db = _uow.DbContext;
            //學習圈內最後一筆活動
            var lastActivityInfo = GetLastActivity(circleKey);
            if (lastActivityInfo == null)
                return result;
            var response =
                 (from rm in db.ActivitysReadMark
                  join a in db.Activitys on rm.ToRoomId equals a.ToRoomId
                  where rm.ToRoomId == circleKey && rm.memberId == memberId &&
                  rm.LastReadActivityIdEnd == a.Id
                  select a).FirstOrDefault();

            //建立一筆已讀資料
            if (response == null)
            {
                var firstActivityInfo = GetFirstActivity(circleKey);
                var entity = new ActivitysReadMark()
                {
                    Enabled = true,
                    LastReadActivityIdBegin = firstActivityInfo.Id,
                    LastReadActivityIdEnd = firstActivityInfo.Id,
                    memberId = memberId,
                    Time = DateTime.UtcNow,
                    ToRoomId = circleKey
                };
                db.ActivitysReadMark.Add(entity);
                db.SaveChanges();
                response = firstActivityInfo;
            }
            return (lastActivityInfo.Id == response.Id) ? result : response.OuterKey;
        }


        public ReadMarkResult<ActivitysViewModel> GetList(string circleKey, int memberId, int maxResult, Guid pageToken)
        {
            var result = new ReadMarkResult<ActivitysViewModel>();
            var list = new List<ActivitysViewModel>();
            try
            {
                var db = _uow.DbContext;
                var memberLastReadActivity = GetMemberLastReadActivity(circleKey, memberId);
                // 取出最新的 maxResult 筆資料
                if (pageToken == null || pageToken == Guid.Empty)
                    if (memberLastReadActivity != Guid.Empty)
                        pageToken = memberLastReadActivity;



                var listResult = _uow.ActivitysRepo.GetInitActivityListByLinQ(circleKey, memberId, maxResult, pageToken);
                if (listResult != null)
                {

                    list = ActivityDetailDataProxy(listResult, maxResult, memberLastReadActivity);
                    result.MaxResult = maxResult;
                    // 更新接口資訊
                    result.Success = true;
                    result.Data = list.ToArray();
                    list = list.OrderBy(x => x.Publish_Date).ToList();
                    var middleActivity = list.FirstOrDefault();
                    var olderResultActivity = db.Activitys.Where(t => t.ToRoomId == circleKey && t.CardisShow == true && t.Publish_Utc < middleActivity.Publish_Utc.Value && t.Id != middleActivity.Id).OrderByDescending(t => t.Publish_Utc).FirstOrDefault();
                    if (olderResultActivity != null && olderResultActivity.Id != middleActivity.Id)
                        result.OlderResultToken = Utility.OuterKeyHelper.GuidToPageToken(olderResultActivity.OuterKey);
                    //為了取得下一個活動資訊
                    list = list.OrderByDescending(t => t.Publish_Date).ToList();
                    var lastActivity = list.FirstOrDefault();
                    var nextPageActivity = db.Activitys.Where(t => t.ToRoomId == circleKey && t.CardisShow == true && t.Publish_Utc > lastActivity.Publish_Utc && t.Id != lastActivity.Id).FirstOrDefault();

                    if (nextPageActivity != null && nextPageActivity.Id != lastActivity.Id)
                    {
                        result.NextPageToken = Utility.OuterKeyHelper.GuidToPageToken(nextPageActivity.OuterKey);
                        result.NewerResultToken = Utility.OuterKeyHelper.GuidToPageToken(nextPageActivity.OuterKey);
                    }
                }
                else
                {
                    result.Success = true;
                    result.Data = new ActivitysViewModel[0];
                    result.Message = "目前無任何活動!!!";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                result.Data = default(ActivitysViewModel[]);
            }

            return result;
        }

        public ReadMarkResult<ActivitysViewModel> GetListByDirection(bool goback, string circleKey, int memberId, int maxResult, Guid pageToken)
        {
            var result = new ReadMarkResult<ActivitysViewModel>();
            var list = new List<ActivitysViewModel>();
            try
            {
                var db = _uow.DbContext;
                // 取出最新的 maxResult 筆資料
                //list = _uow.ActivitysRepo.GetListByDirection(goback, circleKey, memberId, maxResult, pageToken).ToList();
                var listResult = _uow.ActivitysRepo.GetListByLinQ(goback, circleKey, memberId, maxResult, pageToken);
                if (listResult != null)
                {
                    var memberLastReadActivity = GetMemberLastReadActivity(circleKey, memberId);
                    list = ActivityDetailDataProxy(listResult, maxResult, memberLastReadActivity);

                    result.MaxResult = maxResult;
                    // 更新接口資訊
                    result.Success = true;
                    result.Data = list.ToArray();
                    list = list.OrderBy(x => x.Publish_Date).ToList();
                    var middleActivity = list.FirstOrDefault();
                    if (middleActivity != null)
                    {
                        var olderResultActivity = db.Activitys.Where(t => t.ToRoomId == circleKey && t.CardisShow == true && t.Publish_Utc < middleActivity.Publish_Utc && t.Id != middleActivity.Id).OrderByDescending(t => t.Publish_Utc).FirstOrDefault();
                        if (olderResultActivity != null && olderResultActivity.Id != middleActivity.Id)
                            result.OlderResultToken = Utility.OuterKeyHelper.GuidToPageToken(olderResultActivity.OuterKey);
                    }
                    //為了取得下一個活動資訊
                    list = list.OrderByDescending(t => t.Publish_Date).ToList();
                    var lastActivity = list.FirstOrDefault();
                    if (goback == false && lastActivity != null)
                    {
                        var nextPageActivity = db.Activitys.Where(t => t.ToRoomId == circleKey && t.CardisShow == true && t.Publish_Utc > lastActivity.Publish_Utc && t.Id != lastActivity.Id).FirstOrDefault();

                        if (nextPageActivity != null && nextPageActivity.Id != lastActivity.Id)
                        {
                            result.NextPageToken = Utility.OuterKeyHelper.GuidToPageToken(nextPageActivity.OuterKey);
                            result.NewerResultToken = Utility.OuterKeyHelper.GuidToPageToken(nextPageActivity.OuterKey);
                        }
                    }
                }
                else
                {
                    result.Success = true;
                    result.Data = new List<ActivitysViewModel>().ToArray();
                    result.Message = "目前無任何活動!!!";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                result.Data = default(ActivitysViewModel[]);
            }

            return result;
        }

        private ReadMarkResult<ActivitysViewModel> SortList(ReadMarkResult<ActivitysViewModel> result, IEnumerable<ActivitysViewModel> list, SortParam param)
        {

            //以id排序(時間序才不會錯亂)
            var temp = result.Data.ToList();
            if (param.Goback)
                result.Data = temp.OrderByDescending(x => x.Publish_Date).ToArray();//舊訊息:前端UI會往上長，因此是由大到小排序(最後長的是最舊的訊息)
            else
                result.Data = temp.OrderBy(x => x.Publish_Date).ToArray();//新訊息:前端UI是往下長，因此是由小到大排序

            return result;
        }
        

        /// <summary>
        /// 處理查詢活動資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="maxResult"></param>
        /// <returns></returns>
        private List<ActivitysViewModel> ActivityDetailDataProxy(IEnumerable<ActivitysViewModel> datas, int maxResult, Guid eventId)
        {
            var db = _uow.DbContext;
            var list = datas.ToList();
            var index = 0;

            var lastReadActivity = GetByEventId(eventId);
            foreach (var item in list)
            {
                index++;
                item.sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(item.OuterKey);
              //  item.ReadCount = GetActivityReadCount(true, item.OuterKey.ToString());
                item.ReadMark = (lastReadActivity != null && item.Publish_Utc >= lastReadActivity.Publish_Utc) ? false : true;
                if (item.Deleted.Utc.HasValue)
                    item.Deleted_Utc = item.Deleted.Utc.Value;
                if (index == maxResult - 1)
                    item.PositionMark = true;
                if (item.ModuleKey == "message")
                {
                    var msgInfo = db.ActMessage.FirstOrDefault(t => t.EventId == item.OuterKey);
                    item.Text = msgInfo.Content;
                }
            }
            return list;
        }
        public void UpdateRead(string circleKey, int memberId, int? id = 0)
        {
            var db = _uow.DbContext;
            var objRead = db.ActivitysReadMark.FirstOrDefault(x => x.ToRoomId.ToLower() == circleKey.ToLower() && x.memberId == memberId);
            if (objRead != null)
            {
                if (id.HasValue)
                    // 更新最後的一筆已讀
                    objRead.LastReadActivityIdEnd = id.Value;
                objRead.Time = DateTime.UtcNow;
                _uow.SaveChanges();
            }
            else //無已讀資料，就幫使用者建立一筆新的
            {
                var now = DateTime.UtcNow;
                var beginActivityInfo = db.Activitys.Where(t => t.ToRoomId.ToLower() == circleKey).OrderBy(t => t.Id).FirstOrDefault();
                var endActivityInfo = db.Activitys.Where(t => t.ToRoomId.ToLower() == circleKey).OrderByDescending(t => t.Id).FirstOrDefault();
                var actReadMarkEntity = new Infrastructure.Entity.ActivitysReadMark()
                {
                    Enabled = true,
                    LastReadActivityIdBegin = beginActivityInfo.Id,
                    LastReadActivityIdEnd = endActivityInfo.Id,
                    memberId = memberId,
                    ToRoomId = circleKey,
                    Time = now
                };
                db.ActivitysReadMark.Add(actReadMarkEntity);
                db.SaveChanges();
            }
        }
        
        /// <summary>
        /// 取得權限資料
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public JObject CircleAuth(int memberId, int learningId)
        {
            var db = _uow.DbContext;
            //取出所有功能
            var modules = db.Modules.ToList();
            var memberInfo = db.Members.Find(memberId);
            var result = new JObject();
            if (memberInfo == null)
                return null;
            //如果是smartTA，直接給最高權限
            else if (memberInfo.RoleName == "3") {
                result = SetModuleAuths(true);
                return result;
            }
            //查該使用者是否存在學習圈內
            var circleMemberRoleInfo = db.CircleMemberRoleplay.FirstOrDefault(t => t.CircleId == learningId && t.MemberId == memberId);
            if (circleMemberRoleInfo == null)
                return null;
            var roleInfo = db.LearningRole.Find(circleMemberRoleInfo.RoleId);
            if (roleInfo == null)
                return null;
            result = SetModuleAuths(roleInfo.IsAdminRole);
            return result;
        }

        public IQueryable<Activitys> GetActivitys(string circleKey, DateTime leaveDate)
        {
            //取出請假當天的所有點名活動日期
            var cheYear = leaveDate.ToLocalTime().Year;
            var cheMonth = leaveDate.ToLocalTime().Month;
            var cheDay = leaveDate.ToLocalTime().Day;
            //查詢受影響的點名活動
            var signInActivityList = _uow.DbContext.Activitys.Where(t => t.ToRoomId == circleKey && t.StartDate.Value.Year == cheYear && t.ModuleKey == "signIn");
            var compareMonth = signInActivityList.Where(t => t.StartDate.Value.Month == cheMonth);
            var compareDay = compareMonth.Where(t => t.StartDate.Value.Day == cheDay);

            return compareDay;
        }
        /// <summary>
        /// 給予權限
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public JObject SetModuleAuths(bool isAdmin) {
            var result = new JObject();
            //取出所有功能
            var modules = _uow.DbContext.Modules.ToList();
            //所有模組活動
            foreach (var module in modules)
            {
                var resultFunctions = new JObject();
                var functions = _uow.DbContext.ModuleFunction.Where(t => t.ModulesId == module.Id).ToList();
                //取出該模組底下的所有功能 2017-05-11 為了要顯示所有的功能權限狀態 育澍修改
                foreach (var function in functions)
                {
                    //判斷是否有加過
                    if (resultFunctions[function.OutSideKey] == null)
                        resultFunctions.Add(new JProperty(function.OutSideKey,( isAdmin? function.IsAdminAuth : function.IsNormalAuth)));
                }
                result.Add(new JProperty(module.OutSideKey, resultFunctions));
            }
            return result;
        }
    }
}

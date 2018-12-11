using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;

namespace WiicoApi.Service.Backend
{
    public class SyllabusService
    {
        private readonly GenericUnitOfWork _uow = new GenericUnitOfWork();

        public SyllabusService()
        {
            _uow = new GenericUnitOfWork();
        }

        #region 課程大綱管理

        /// <summary>
        /// 取得課綱列表 -APP專用
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.CourseManage.GetCourseSyllabusResponse> APPGetCourSyllabus(string token, string circleKey)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token);
            var response = new List<Infrastructure.ViewModel.CourseManage.GetCourseSyllabusResponse>();
            if (checkToken == null)
                return null;
            var datas = GetSyllabusList(circleKey);
            if (datas == null)
                return null;

            foreach (var data in datas)
            {
                var syllabusData = new GetCourseSyllabusResponse()
                {
                    Id = data.Id,
                    SyllSort = data.Sort,
                    SyllTitle = data.Name,
                    SyllNote = data.Note,
                    Syll_date = data.Syll_Date.ToLocalTime(),
                    Syll_id = data.Syll_Guid.ToString()
                };
                var activitys = GetActivitySyllabusData(data.Id);
                if (activitys != null)
                    syllabusData.ActivityList = activitys;
                else
                    syllabusData.ActivityList = new List<ActivitySyllabusResponseData>();
                response.Add(syllabusData);
            }
            return response;
        }
        /// <summary>
        /// 取得進度課綱 - 後端用
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Syllabus> GetSyllabusList(string circleKey)
        {
            var db = _uow.DbContext;
            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == circleKey.ToLower());
            if (learningCircleInfo == null)
                return null;
            return GetSyllabus(learningCircleInfo.Id, false);
        }

        /// <summary>
        /// 取出課程大綱列表
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public IEnumerable<Syllabus> GetSyllabus(int learningCircleId, bool isDelete)
        {
            var db = _uow.DbContext;
            var result = (isDelete)
                         ? ((from sb in db.Syllabus
                             join lc in db.LearningCircle on sb.Course_No equals lc.LearningOuterKey
                             where lc.Id == learningCircleId && sb.Enable == false && sb.Visibility == false && sb.DeleteUser != null
                             orderby sb.Syll_Date
                             select sb).ToList())
                          : ((from sb in db.Syllabus
                              join lc in db.LearningCircle on sb.Course_No equals lc.LearningOuterKey
                              where lc.Id == learningCircleId && sb.Enable == true && sb.Visibility == true && sb.DeleteUser == null
                              orderby sb.Syll_Date
                              select sb).ToList());
            foreach (var data in result)
            {
                data.Syll_Date = data.Syll_Date.ToLocalTime();
            }
            return result;
        }

        /// <summary>
        /// 取得課綱資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public IEnumerable<Syllabus> GetSyllabusByCircleKey(string circleKey, bool isDelete)
        {
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey);
            var result = GetSyllabus(learningCircleInfo.Id, false);
            return result;
        }

        /// <summary>
        /// 組出前端刪除課程的列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public string GetDeleteListString(string circleKey)
        {
            var result = string.Empty;
            var learningCircleInfo = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == circleKey);
            if (learningCircleInfo == null)
                return null;

            var datas = GetSyllabus(learningCircleInfo.Id, true);
            foreach (var _item in datas.ToList())
            {
                result += "<tr><td>課程大綱名稱：" + _item.Name + "    |   日期：" + _item.Syll_Date + "</td><td><a href='#' id='recovery' data-syllabusId='" + _item.Id + "'>復原</a></td></tr>";
            }
            return result;
        }

        /// <summary>
        /// 新增課綱
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="syllDate"></param>
        /// <param name="sort"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Syllabus InsertSyllabusByToken(string circleKey, string name, string note, DateTime syllDate, string token)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            var result = InsertSyllabus(circleKey, name, note, syllDate.ToString("yyyy-MM-ddTHH:mm:ss"), tokenInfo.MemberId);
            UpdateSort(result.Course_No.ToLower());
            return result;
        }

        /// <summary>
        /// 編輯課綱
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="syllDate"></param>
        /// <param name="sort"></param>
        /// <param name="enable"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Syllabus UpdateSyllabusByToken(int syllabusId, string name, string note, DateTime syllDate, string token)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            var result = UpdateSyllabus(syllabusId.ToString(), name, note, syllDate.ToString("yyyy-MM-ddTHH:mm:ss"), tokenInfo.Id);
            UpdateSort(result.Course_No.ToLower());
            return result;
        }

        /// <summary>
        /// 編輯課綱
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="syllDate"></param>
        /// <param name="sort"></param>
        /// <param name="enable"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Syllabus DeleteSyllabusByToken(int syllabusId, string token)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            var result = DeleteSyllabus(syllabusId, tokenInfo.MemberId);
            UpdateSort(result.Course_No.ToLower());
            return result;
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        public void UpdateSort(string circleKey)
        {
            var db = _uow.DbContext;
            var datas = db.Syllabus.Where(t => t.Course_No.ToLower() == circleKey).OrderBy(t => t.Syll_Date);
            if (datas.Count() > 0)
            {
                var sort = 0;
                foreach (var data in datas)
                {
                    data.Sort = sort;
                    sort++;
                }
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 新增多筆課程章節
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="datas"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Syllabus> SyllabusesDataProxy(SyllabusManagePostRequest data)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(data.Token).Result;
            if (tokenInfo == null)
                return null;
            var db = _uow.DbContext;
            try
            {
                //查出新增的資料
                var insertDatas = (from d in data.Syllabuses
                                   where d.Id == null || d.Id <= 0
                                   select new Syllabus
                                   {
                                       Course_No = data.CircleKey.ToLower(),
                                       Created = TimeData.Create(DateTime.UtcNow),
                                       Deleted = TimeData.Create(null),
                                       Updated = TimeData.Create(null),
                                       CreateUser = tokenInfo.MemberId,
                                       Enable = true,
                                       Name = d.Title,
                                       Note = d.Note,
                                       Syll_Date = d.Date.ToUniversalTime(),
                                       Visibility = true
                                   }).OrderBy(t => t.Syll_Date).ToList();
                var insertSort = 0;
                //整理新增資料
                foreach (var insertData in insertDatas)
                {
                    var guid = Guid.NewGuid();
                    insertData.Sort = insertSort;
                    insertData.Syll_Guid = guid;
                    insertSort++;
                }
                db.Syllabus.AddRange(insertDatas);

                //查出要編輯的資料
                var updateDatas = data.Syllabuses.Where(t => t.Id.HasValue || t.Id > 0).ToList();

                foreach (var updateData in updateDatas)
                {
                    var dbData = db.Syllabus.Find(updateData.Id);
                    dbData.Name = updateData.Title;
                    dbData.Note = updateData.Note;
                    dbData.Syll_Date = updateData.Date.ToUniversalTime();
                    dbData.Updated = TimeData.Create(DateTime.UtcNow);
                    dbData.UpdateUser = tokenInfo.MemberId;
                    dbData.Sort = updateData.Sort;
                }
                //比對需要刪除資料的查詢
                var checkDatas = (from sy in db.Syllabus
                                  where sy.Course_No == data.CircleKey
                                  select sy.Id).ToList();
                //取得聯集
                var unionDatas = checkDatas.Union(data.Syllabuses.Where(t => t.Id.HasValue).Select(t => t.Id.Value));
                //取得要刪除的資料群
                var deleteDatas = unionDatas.Except(data.Syllabuses.Where(t => t.Id.HasValue).Select(t => t.Id.Value));
                foreach (var deleteData in deleteDatas)
                {
                    var removeData = db.Syllabus.Find(deleteData);
                    if (removeData == null)
                        continue;
                    db.Syllabus.Remove(removeData);
                }
                db.SaveChanges();
                //重新計算排序
                UpdateSort(data.CircleKey.ToLower());
                var responseData = db.Syllabus.Where(t => t.Course_No == data.CircleKey.ToLower()).ToList();
                return responseData;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        /// <summary>
        /// 新增課程大綱
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="syllDate"></param>
        /// <param name="sort"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public Syllabus InsertSyllabus(string circleKey, string name, string note, string syllDate, int creator)
        {
            try
            {
                var db = _uow.DbContext;
                var entity = new Syllabus()
                {
                    Name = name,
                    Enable = true,
                    Visibility = true,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Course_No = circleKey,
                    CreateUser = creator,
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    Note = note,
                    Syll_Date = Convert.ToDateTime(syllDate).ToUniversalTime(),
                    Syll_Guid = Guid.NewGuid()
                };
                db.Syllabus.Add(entity);

                db.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        /// <summary>
        /// 更新課程大綱
        /// </summary>
        /// <param name="syllabusId"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="syllDate"></param>
        /// <param name="sort"></param>
        /// <param name="updater"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public Syllabus UpdateSyllabus(string syllabusId, string name, string note, string syllDate, int updater)
        {
            try
            {
                var db = _uow.DbContext;
                var id = Convert.ToInt32(syllabusId);
                var entity = db.Syllabus.FirstOrDefault(t => t.Id == id);
                if (entity == null)
                    return null;
                else
                {
                    entity.Name = name;
                    entity.Note = note;
                    entity.Syll_Date = Convert.ToDateTime(syllDate).ToUniversalTime();
                    entity.Syll_Guid = Guid.NewGuid();
                    entity.Updated = TimeData.Create(DateTime.UtcNow);
                    entity.Deleted = TimeData.Create(null);
                    entity.UpdateUser = updater;



                    db.SaveChanges();
                }
                return entity;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public Syllabus DeleteSyllabus(int syllabusId, int deleter)
        {
            var db = _uow.DbContext;
            var entity = db.Syllabus.Find(syllabusId);
            if (entity == null)
                return null;
            else
            {
                entity.Deleted = TimeData.Create(DateTime.UtcNow);
                entity.DeleteUser = deleter;
                entity.Enable = false;
                entity.Visibility = false;
                db.SaveChanges();
                return entity;
            }
        }
        /// <summary>
        /// 復原刪除
        /// </summary>
        /// <param name="syllabusId"></param>
        /// <returns></returns>
        public Syllabus RecoverySyllabus(int syllabusId)
        {
            var db = _uow.DbContext;
            var entity = db.Syllabus.Find(syllabusId);
            if (entity == null)
                return null;
            else
            {
                entity.Deleted = TimeData.Create(null);
                entity.DeleteUser = null;
                entity.Enable = true;
                entity.Visibility = true;
                db.SaveChanges();
                return entity;
            }
        }

        #endregion


        #region 活動與課綱關聯管理

        /// <summary>
        /// 取得活動與進度的關聯列表
        /// </summary>
        /// <param name="syllabusId"></param>
        /// <returns></returns>
        public IEnumerable<ActivitySyllabusResponseData> GetActivitySyllabusData(int syllabusId)
        {
            var db = _uow.DbContext;
            var datas = (from ats in db.ActivitySyllabus
                         join a in db.Activitys on ats.ActivityId equals a.Id
                         where ats.SyllabusId == syllabusId
                         select new ActivitySyllabusResponseData()
                         {
                             ModuleKey = a.ModuleKey,
                             EventId = a.OuterKey
                         }).ToList();
            if (datas.FirstOrDefault() == null)
                return null;
            foreach (var data in datas)
            {
                data.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(data.EventId);
                switch (data.ModuleKey.ToLower())
                {
                    case "discussion":
                        var discussionInfo = db.ActDiscussion.FirstOrDefault(t => t.EventId == data.EventId);
                        if (discussionInfo == null)
                            continue;
                        data.Title = discussionInfo.Name;
                        break;
                    case "signin":
                        var signInfo = db.ActRollCall.FirstOrDefault(t => t.EventId == data.EventId);
                        if (signInfo == null)
                            continue;
                        data.Title = signInfo.Name;
                        break;
                    case "upload":
                        var homeworkInfo = db.ActHomeWork.FirstOrDefault(t => t.EventId == data.EventId);
                        if (homeworkInfo == null)
                            continue;
                        data.Title = homeworkInfo.Name;
                        break;
                    case "vote":
                        var voteInfo = db.ActVote.FirstOrDefault(t => t.EventId == data.EventId);
                        if (voteInfo == null)
                            continue;
                        data.Title = voteInfo.Title;
                        break;
                    case "leave":
                        var leaveInfo = db.AttendanceLeave.FirstOrDefault(t => t.EventId == data.EventId);
                        if (leaveInfo == null)
                            continue;
                        data.Title = leaveInfo.Subject;
                        break;
                    case "message":
                        var msgInfo = db.ActMessage.FirstOrDefault(t => t.EventId == data.EventId);
                        if (msgInfo == null)
                            continue;
                        data.Title = msgInfo.Content;
                        break;
                    case "material":
                        var materialInfo = db.ActMaterial.FirstOrDefault(t => t.EventId == data.EventId);
                        if (materialInfo == null)
                            continue;
                        data.Title = materialInfo.Name;
                        break;
                    default:
                        break;
                }
            }
            return datas.ToList();
        }

        /// <summary>
        /// 建立活動與進度的關聯
        /// </summary>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
        /// <param name="syllabusId"></param>
        /// <returns></returns>
        public bool CreateActivitySyllabusData(string outerKey, int syllabusId)
        {
            var db = _uow.DbContext;
            var activityService = new ActivityService();
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (eventId.HasValue == false)
                return false;
            var activityInfo = activityService.GetByEventId(eventId.Value);
            if (activityInfo == null)
                return false;
            var entity = new ActivitySyllabus() { ActivityId = activityInfo.Id, SyllabusId = syllabusId };
            try
            {
                db.ActivitySyllabus.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 編輯活動與進度課綱的關聯
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="syllabusId"></param>
        /// <returns></returns>
        public bool UpdateActivitySyllabusData(string outerKey, int syllabusId)
        {
            var db = _uow.DbContext;
            var activityService = new ActivityService();
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (eventId.HasValue == false)
                return false;
            var activityInfo = activityService.GetByEventId(eventId.Value);

            if (activityInfo == null)
                return false;
            var entity = db.ActivitySyllabus.FirstOrDefault(t => t.ActivityId == activityInfo.Id);
            if (entity == null)
                return false;
            try
            {
                entity.SyllabusId = syllabusId;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 刪除活動與進度課綱的關聯
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="syllabusId"></param>
        /// <returns></returns>
        public bool DeleteActivitySyllabusData(string outerKey, int syllabusId)
        {
            var db = _uow.DbContext;
            var activityService = new ActivityService();
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (eventId.HasValue == false)
                return false;
            var activityInfo = activityService.GetByEventId(eventId.Value);
            if (activityInfo == null)
                return false;
            var entity = db.ActivitySyllabus.FirstOrDefault(t => t.ActivityId == activityInfo.Id);
            if (entity == null)
                return false;
            try
            {
                db.ActivitySyllabus.Remove(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Repository;

namespace WiicoApi.Service.SignalRService.SignIn
{
    public class SignInLogService
    {
        private readonly GenericUnitOfWork _uow;

        public SignInLogService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 根據點名活動編號與學生編號與狀態取得點名資料
        /// </summary>
        /// <param name="actRollCallId"></param>
        /// <param name="studId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Infrastructure.Entity.ActRollCallLog GetLogData(int actRollCallId,int studId,string status) {
            return _uow.DbContext.ActRollCallLog.FirstOrDefault(t => t.RollCallId == actRollCallId && t.StudId == studId && t.Status == status);
        }

        /// <summary>
        /// 根據點名編號與學生編號取得點名資料
        /// </summary>
        /// <param name="actRollCallId"></param>
        /// <param name="studId"></param>
        /// <returns></returns>
        public List<ActRollCallLog> GetLogList(int actRollCallId) {
            var responseData = _uow.DbContext.ActRollCallLog.Where(t => t.RollCallId == actRollCallId).ToList();
            if (responseData.FirstOrDefault() == null)
                return null;
            else
                return responseData;
        }
        /// <summary>
        /// 取得點名資訊
        /// </summary>
        /// <param name="eventIds">活動 eventId</param>
        /// <param name="memberId">查詢指定人員(id)簽到紀錄，若傳入null表示查詢所有人</param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ValueObject.SignInEvent> GetSignInData(SignInEventParam param)
        {
            var isSce = Convert.ToBoolean(ConfigurationManager.AppSettings["isSce"].ToString());
            var list = _uow.ActivitysRepo.GetSignInData(param, isSce);

            #region //整理回傳資訊

            var signIn = from l in list
                         group l by new
                         {
                             l.Id,
                             l.ToRoomId,
                             l.ModuleKey,
                             l.OuterKey,
                             l.CreatorAccount,
                             l.CreatorPhoto,
                             l.CreatorName,
                             l.Created_Utc,
                             l.Updated_Utc,
                             l.Deleted_Utc,
                             l.ActivityDate,
                             l.StartDate,
                             l.Duration,
                             l.IsNewest,
                             l.ActivityId,
                             l.Name,
                             l.SignInKey,
                             l.SignInPwd,
                             l.Publish_Utc
                         }
                         into g
                         select new Infrastructure.ValueObject.SignInEvent
                         {

                             Id = g.Key.Id,
                             ToRoomId = g.Key.ToRoomId,
                             ModuleKey = g.Key.ModuleKey,
                             Text = "", //訊息專用欄位，點名預塞空字串
                             OuterKey = g.Key.OuterKey,
                             CreatorAccount = g.Key.CreatorAccount,
                             CreatorName = g.Key.CreatorName,
                             Created_Utc = g.Key.Created_Utc,
                             CreatorPhoto = g.Key.CreatorPhoto,
                             Updated_Utc = g.Key.Updated_Utc,
                             Deleted_Utc = g.Key.Deleted_Utc,
                             ActivityDate = g.Key.ActivityDate,
                             StartDate = g.Key.StartDate,
                             Duration = g.Key.Duration,
                             IsNewest = g.Key.IsNewest,
                             ActivityId = g.Key.ActivityId,
                             Name = g.Key.Name,
                             SignInPwd = g.Key.SignInPwd,
                             SignInKey = g.Key.SignInKey,
                             Publish_Utc = g.Key.Publish_Utc,
                             Logs = (from k in list
                                     orderby k.Sort // UI預設是ican5排序
                                     where k.OuterKey == g.Key.OuterKey
                                     select new Infrastructure.ValueObject.SignInLog
                                     {
                                         LeaveEventId = k.LeaveEventId,
                                         Sort = k.Sort,
                                         StuId = k.StuId,
                                         StudId = k.StudId,
                                         StudName = k.StudName,
                                         StudPhoto = k.StudPhoto,
                                         Status = k.Status,
                                         LeaveStatus = k.LeaveStatus,
                                         Time = Convert.ToDateTime(k.Time).ToLocalTime(),
                                         CreatorAccount = k.LogCreator,
                                         UpdateDate_Utc = k.LogUpdateDate
                                     }).ToArray()
                         };
            #endregion

            //分頁
            if (param.Pages.HasValue && param.Rows.HasValue)
            {
                int skipRows = (param.Pages.Value - 1) * param.Rows.Value;
                signIn = signIn.OrderByDescending(t => t.Publish_Utc).Skip(skipRows).Take(param.Rows.Value);

            }
            return signIn;
        }

        /// <summary>
        /// 取得單一點名紀錄
        /// </summary>
        /// <returns></returns>
        public Infrastructure.ValueObject.SignInLog GetSignInLog(int memId, Guid eventId, int studId)
        {
            var db = _uow.DbContext;
            var rtn = new Infrastructure.ValueObject.SignInLog();
            var dt = DateTime.UtcNow;
            var rollCallInfo = db.ActRollCall.FirstOrDefault(x => x.EventId == eventId);
            var act = db.Activitys.FirstOrDefault(x => x.OuterKey == eventId);
            var log = db.ActRollCallLog.FirstOrDefault(x => x.RollCallId == rollCallInfo.Id && x.StudId == studId);
            if (log == null)
                return null;
            var learningInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == act.ToRoomId);
            var oldState = log.Status;
            var _leaveInfo = db.AttendanceLeave.Where(t => t.StudId == studId && t.LearningId == learningInfo.Id).ToList();
            if (_leaveInfo == null)
                return null;
            if (act.StartDate != null)
            {
                var leaveInfo = _leaveInfo.Where(t => t.LeaveDate.ToLocalTime().Date == act.StartDate.Value.ToLocalTime().Date);
                // var leaveInfoMonth = leaveinfoYear.Where(t => t.LeaveDate.Month == act.StartDate.Value.Month);
                // var leaveInfo = leaveInfoMonth.Where(t => t.LeaveDate.Day == act.StartDate.Value.Day);
                if (leaveInfo != null)
                {
                    foreach (var _info in leaveInfo)
                    {
                        if (rtn.LeaveStatus == "10")
                        {
                            break;
                        }
                        else if (_info.Status == "10")
                        {
                            rtn.LeaveStatus = _info.Status;
                        }
                        else if (rtn.LeaveStatus != "10" && _info.Status == "20")
                        {
                            rtn.LeaveStatus = _info.Status;
                        }
                        else if (rtn.LeaveStatus == null)
                        {
                            rtn.LeaveStatus = _info.Status;
                        }

                        rtn.LeaveEventId = _info.EventId;
                    }
                }
            }

            rtn.StuId = studId;
            //申請人資訊
            var stu = db.Members.Find(studId);
            //查詢人員資訊
            var creator = db.Members.FirstOrDefault(x => x.Id == log.CreateUser);
            rtn.StudId = stu.Account;
            rtn.StudName = stu.Name;
            //rtn.StudPhoto = _uow.IThinkVmRepo.GetSingleMember(stu.Account).Photo;
            rtn.StudPhoto = stu.Photo;
            rtn.Status = log.Status;
            rtn.OldState = int.Parse(oldState);
            rtn.Time = log.Time;
            rtn.CreatorAccount = creator.Account;
            rtn.UpdateDate_Utc = dt;
            return rtn;
        }

        /// <summary>
        /// 更新點名活動簽到狀態
        /// </summary>
        /// <param name="memId"></param>
        /// <param name="eventId"></param>
        /// <param name="studId"></param>
        /// <param name="status"></param>
        public Infrastructure.ValueObject.SignInLog UpdateLog(int memId, Guid eventId, int studId, string status)
        {
            var dt = DateTime.UtcNow;
            var rtn = new Infrastructure.ValueObject.SignInLog();

            try
            {
                #region //1. 更新點名log
                var db = _uow.DbContext;
                var rollCallInfo = db.ActRollCall.FirstOrDefault(x => x.EventId == eventId);
                var log = db.ActRollCallLog.FirstOrDefault(x => x.RollCallId == rollCallInfo.Id && x.StudId == studId);
                if (log == null)
                {
                    var newLogEntity = new ActRollCallLog()
                    {
                        RollCallId = rollCallInfo.Id,
                        StudId = studId,
                        Status = status,
                        Created = TimeData.Create(DateTime.UtcNow),
                        Deleted = TimeData.Create(null),
                        Updated = TimeData.Create(null),
                        CreateUser = memId
                    };
                    var newAttendanceRecord = new AttendanceRecord()
                    {
                        LearningId = rollCallInfo.LearningId,
                        EventId = rollCallInfo.EventId,
                        StudId = studId,
                        Status = status,
                        UpdateTime = DateTime.UtcNow
                    };

                    db.AttendanceRecord.Add(newAttendanceRecord);
                    db.ActRollCallLog.Add(newLogEntity);
                    log = newLogEntity;
                }
                var oldState = log.Status;

                if (log.Time.HasValue == false) log.Time = dt; //第一次因參與活動而改變狀態的時間
                log.Status = status;
                log.UpdateUser = memId;
                log.Updated = TimeData.Create(dt);

                #endregion

                #region //2. 更新核心出缺勤

                var attend = db.AttendanceRecord.FirstOrDefault(x => x.EventId == eventId && x.StudId == studId);
                if (attend != null)
                {
                    attend.Status = status;
                    attend.UpdateTime = dt;
                }

                #endregion

                #region //2017-01-09 add by sophiee 狀態變更，也更新活動最後編輯時間

                var act = db.Activitys.FirstOrDefault(x => x.OuterKey == eventId);
                if (act != null)
                {
                    act.UpdateUser = memId;
                    act.Updated = Infrastructure.Property.TimeData.Create(dt);

                }


                #endregion

                //查詢人員資訊
                var creator = db.Members.FirstOrDefault(x => x.Id == log.CreateUser);
                // var stu = db.Members.Where(x => x.Id == studId).SingleOrDefault();
                if (creator != null)
                    rtn.CreatorAccount = creator.Account;

                //執行
                db.SaveChanges();
                var stu = db.Members.FirstOrDefault(t => t.Id == studId);
                #region // 3.回傳資訊
                if (stu != null)
                {
                    rtn.StuId = studId;
                    rtn.StudId = stu.Account;
                    rtn.StudName = stu.Name;
                    rtn.StudPhoto = stu.Photo;
                }

                var learningInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == act.ToRoomId);
                if (learningInfo != null)
                {
                    var leaveinfoYear = db.AttendanceLeave.Where(t => t.StudId == studId && t.LearningId == learningInfo.Id && t.LeaveDate.Year == act.StartDate.Value.Year);
                    var leaveInfoMonth = leaveinfoYear.Where(t => t.LeaveDate.Month == act.StartDate.Value.Month);
                    var leaveInfo = leaveInfoMonth.Where(t => t.LeaveDate.Day == act.StartDate.Value.Day);
                    if (leaveInfo != null)
                    {
                        var _index = 0;
                        foreach (var _leave in leaveInfo)
                        {
                            if (rtn.LeaveStatus == "10")
                                break;
                            else if (_leave.Status == "10")
                                rtn.LeaveStatus = _leave.Status;
                            else if (_index < leaveInfo.Count() && _leave.Status == "20")
                                rtn.LeaveStatus = _leave.Status;
                            else if (_index < leaveInfo.Count() && rtn.LeaveStatus == null)
                                rtn.LeaveStatus = _leave.Status;

                            rtn.LeaveEventId = _leave.EventId;
                            _index++;
                        }
                    }
                }
                rtn.Status = status;

                rtn.OldState = int.Parse(oldState);
                rtn.Time = log.Time;

                rtn.UpdateDate_Utc = dt;


                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return rtn;
        }
    }
}

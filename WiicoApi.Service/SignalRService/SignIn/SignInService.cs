using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.SignIn;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.Service.SignalRService.SignIn
{
    public class SignInService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly CacheService cacheService = new CacheService();
        private readonly SignInLogService signInLogService = new SignInLogService();

        public SignInService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得資料表資料
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Infrastructure.Entity.ActRollCall GetSignInInfomation(Guid eventId)
        {
            var responseData = _uow.DbContext.ActRollCall.FirstOrDefault(t => t.EventId == eventId);
            return responseData;
        }
        /// <summary>
        /// 長出點名活動列表資料
        /// </summary>
        /// <param name="memberId">參與者編號</param>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="pages">查詢索引</param>
        /// <param name="rows">查詢數量</param>
        /// <returns></returns>
        public IEnumerable<SignInModuleList> GetSignInList(int memberId, string circleKey, int? pages, int? rows)
        {
            var activityService = new ActivityService();
            var authService = new AuthService();
            var acts = activityService.GetActivityListByModuleKey(circleKey, Utility.ParaCondition.ModuleType.SignIn, pages, rows);
            var learningCircieService = new LearningCircleService();
            var resultList = new List<SignInModuleList>();
            var learningCircleInfo = learningCircieService.GetDetailByOuterKey(circleKey.ToLower());
            var memberService = new MemberService();
            if (learningCircleInfo != null)
            {
                var IsTeacher = authService.CheckFunctionAuth(learningCircleInfo.Id, "signInadmin", memberId);
                //取得學習圈所有人

                foreach (var act in acts)
                {
                    var item = new SignInModuleList();
                    //狀態列表
                    item.SignInCount = IsTeacher ? GetSignInStatusLog(act.OuterKey, memberService.GetStudentList(circleKey).Count()) : new List<Infrastructure.ValueObject.SignStatus>();
                    item.MyStatus = IsTeacher ? 0 : GetStudentStatus(memberId, act.OuterKey);
                    if (act.ActivityDate.HasValue)
                        item.ActivityTime = act.ActivityDate.Value.ToLocalTime();
                    if (act.Created.Utc.HasValue)
                        item.CreateTime = act.Created.Local.Value;
                    if (act.Publish_Utc.HasValue)
                        item.Publish_date = act.Publish_Utc.Value.ToLocalTime();
                    if (act.StartDate.HasValue)
                        item.StartDate = act.StartDate.Value.ToLocalTime();
                    if (act.Duration.HasValue)
                        item.Duration = act.Duration.Value;

                    item.GroupId = circleKey;
                    item.ModuleKey = Utility.ParaCondition.ModuleType.SignIn;
                    item.OuterKey = Service.Utility.OuterKeyHelper.GuidToPageToken(act.OuterKey);
                    if (act.StartDate.HasValue && act.Duration.HasValue)
                    {
                        var endDate = act.StartDate.Value.Add(new TimeSpan(0, 0, act.Duration.Value));
                        item.RemainSeconds = (endDate - DateTime.UtcNow).TotalSeconds;
                    }
                    resultList.Add(item);
                }
            }
            return resultList;
        }
        /// <summary>
        /// 取得點名狀態列表
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberCount"></param>
        /// <returns></returns>
        public List<Infrastructure.ValueObject.SignStatus> GetSignInStatusLog(Guid eventId, int memberCount)
        {
            var db = _uow.DbContext;
            var result = new List<Infrastructure.ValueObject.SignStatus>();
            //用於計算目前參與人數
            var checkMemberCount = 0;
            var logResult = (from rcl in db.ActRollCallLog
                             join rc in db.ActRollCall on rcl.RollCallId equals rc.Id
                             where rc.EventId == eventId
                             select rcl).GroupBy(t => t.Status);
            var statusList = SetEmptyStatus();
            foreach (var log in logResult)
            {
                var status = Convert.ToInt32(log.Key).ToString();
                var item = statusList.FirstOrDefault(t => t.Value == status);
                item.Count = log.Count();
                checkMemberCount += log.Count();
            }
            //參與人數少於全班學生人數，塞[未開放您參加此活動]的數量 =  memberCount(全班學生人數) - checkMemberCount(參與人數)
            if (checkMemberCount < memberCount)
                statusList.FirstOrDefault(t => t.Value == "-1").Count = memberCount - checkMemberCount;
            //查看
            result = statusList;
            return result;
        }
        /// <summary>
        /// 取得該學生在某次點名的狀態
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public int GetStudentStatus(int memberId, Guid eventId)
        {
            var db = _uow.DbContext;
            var result = (from rcl in db.ActRollCallLog
                          join rc in db.ActRollCall on rcl.RollCallId equals rc.Id
                          where rc.EventId == eventId && rcl.StudId == memberId
                          select rcl).FirstOrDefault();
            return result != null ? Convert.ToInt32(result.Status) : -1;
        }
        /// <summary>
        /// 塞Count = 0的狀態列表
        /// </summary>
        /// <returns></returns>
        private List<Infrastructure.ValueObject.SignStatus> SetEmptyStatus()
        {
            var result = new List<Infrastructure.ValueObject.SignStatus>();

            var statusKeyArray = System.Enum.GetValues(typeof(Infrastructure.ValueObject.AttendanceStateEnum)).Cast<Infrastructure.ValueObject.AttendanceStateEnum>().ToList(); // 從 enum 取得資料當作 key
            foreach (var status in statusKeyArray)
            {
                var item = new Infrastructure.ValueObject.SignStatus();
                var value = Convert.ToInt32(status);
                item.Count = 0;
                item.Name = status.ToString();
                item.Value = value.ToString();
                result.Add(item);
            }
            return result;
        }
        public Infrastructure.ValueObject.SignInEvent GetSignInEvent(Guid eventId, int? memberId = null)
        {
            var db = _uow.DbContext;
            var activityInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);

            if (activityInfo != null)
            {
                if (activityInfo.CardisShow == true)
                {
                    memberId = memberId.HasValue ? memberId.Value : activityInfo.CreateUser;
                    var memberInfo = db.Members.Find(memberId);
                    //取得課內權限
                    var checkMemberAuth = (from cmr in db.CircleMemberRoleplay
                                           join lr in db.LearningRole on cmr.RoleId equals lr.Id
                                           join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                                           where cmr.MemberId == memberId.Value && lc.LearningOuterKey.ToLower() == activityInfo.ToRoomId.ToLower()
                                           select lr).FirstOrDefault();
                    //如果沒有課內權限，也不是SmartTA
                    if (checkMemberAuth==null && memberInfo.RoleName!="3")
                        return null;
                    //如果沒有課內權限，那一定是smartTA，所以true
                    var isAdminRole = checkMemberAuth!=null ? checkMemberAuth.IsAdminRole : true;

                    var param = new Infrastructure.DataTransferObject.SignInEventParam() { IsAdminRole = isAdminRole, EventIds = new List<Guid>() { eventId }, MemberId = memberId.Value, CircleKey = activityInfo.ToRoomId, NotDeleted = true, Pages = 1, Rows = 20 };

                    return GetMutipEventData(param,isAdminRole).FirstOrDefault();
                }
                else
                    return null;
            }
            else
                return null;
        }

        public List<Infrastructure.ValueObject.SignInEvent> GetMutipEventData(Infrastructure.DataTransferObject.SignInEventParam param, bool IsAdminRole)
        {
            if (!string.IsNullOrEmpty(param.CircleKey))
            {
                //int learningId = cacheService.GetCircle(param.CircleKey).Id;
            }
            else
            {
                // 目前活動都是在同一個學習圈內發生，直接以第一個點名活動來判斷是否有管理權限
                var eventId = param.EventIds.First();
                var objEvent = _uow.EntityRepository<ActRollCall>().GetFirst(x => x.EventId == eventId);
            }

            // 取得點名活動
            var list = signInLogService.GetSignInData(param).ToList();

            //有管理權限，統計各狀態數量
            if (IsAdminRole)
            {
                foreach (var data in list)
                {
                    data.sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(data.OuterKey);
                    var counts = data.Logs.GroupBy(x => x.Status);
                    var status = Utility.ParaCondition.AttendanceState.Status;
                    foreach (var s in status)
                    {
                        var sum = new Infrastructure.ValueObject.SignStatus { Name = s.Value, Value = s.Key, Count = 0 };
                        var log = counts.SingleOrDefault(x => x.Key == s.Key);
                        if (log != null)
                            sum.Count = log.Count();

                        data.SignInCount.Add(sum);
                    }
                }
            }
            else
            {
                foreach (var signIn in list)
                {
                    //非管理者，不顯示手動簽到驗證碼
                    signIn.SignInPwd = "";
                    signIn.sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(signIn.OuterKey);
                    //是否參與該次活動
                    if (signIn.Logs.FirstOrDefault().Status == Utility.ParaCondition.AttendanceState.NoNeed)
                    {
                        signIn.IsNoAuth = true;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 建立一筆點名活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="circleId"></param>
        /// <param name="memberId"></param>
        /// <param name="beaconKey"></param>
        /// <param name="duration"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActivitysViewModel InsertSignInData(string circleKey, int circleId, int memberId, string beaconKey, int duration, string name) {
            var responseData = _uow.SignInRepo.InsertSignIn(circleKey, circleId, memberId, beaconKey, duration, name);
            var insertSignInLogData = InsertSignInLogData(circleId, responseData.OuterKey, memberId, responseData.RollCallId);
            if (insertSignInLogData) {
                UpdateStartDate(responseData.OuterKey, memberId, true);
                return responseData;
            }
            else
                return null;
        }

        /// <summary>
        /// 新增點名活動成員資料
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="eventId"></param>
        /// <param name="creator"></param>
        /// <param name="rollCallId"></param>
        /// <returns></returns>
        public bool InsertSignInLogData(int circleId,Guid eventId,int creator, int rollCallId ) {
            var db = _uow.DbContext;
            // 2.1 取得所有應該要被點名的成員
             var memberIds = (from m in db.Members
                             join cmr in db.CircleMemberRoleplay on m.Id equals cmr.MemberId
                             join lr in db.LearningRole on cmr.RoleId equals lr.Id
                             where cmr.CircleId == circleId && lr.IsAdminRole == false
                             select m.Id).ToArray();
            var attendanceRecordService = new AttendanceRecordService();
            // 2.2 整理資料-點名紀錄
            var attendances = attendanceRecordService.GetInitAttendanceRecords(circleId, eventId, memberIds);
            // 2.3 整理資料-核心出缺勤資料表
            var logs = GetInitLogs(creator, memberIds);
            //2.1 更新點名活動id，新增點名紀錄
            logs.ForEach(x => x.RollCallId = rollCallId);
            //2.1.1檢查是否有請假資訊
            var newLog = CheckLeaveInfo(DateTime.UtcNow, logs, circleId);
            try {
                db.ActRollCallLog.AddRange(newLog);
                //2.2 新增核心出缺勤紀錄
                db.AttendanceRecord.AddRange(attendances);
                db.SaveChanges();
                //執行
                return true;
            } catch (Exception ex) {
                return false;
                throw ex;
            }

        }


        private string GetRandomPwd(RandomPwd type, int length)
        {
            Random random = new Random();
            string chars = "";

            switch (type)
            {
                case RandomPwd.Number:
                    chars = "0123456789";
                    break;
                case RandomPwd.Uppercase:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case RandomPwd.Lowercase:
                    chars = "abcdefghijklmnopqrstuvwxyz";
                    break;
            }

            var data = Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray();

            var rtn = new string(data);

            return rtn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public string UpdateSignInPwd(Guid eventId, int memberId)
        {
            var rc = _uow.DbContext.ActRollCall.FirstOrDefault(x => x.EventId == eventId);
            if (rc == null)
                return null;
            string newPwd = GetRandomPwd(RandomPwd.Number, 4);

            rc.SignInPwd = newPwd;
            rc.UpdateUser = memberId;
            rc.Updated = TimeData.Create(DateTime.UtcNow);

            _uow.DbContext.SaveChanges();

            return newPwd;
        }

        /// <summary>
        /// 確認請假資訊 - 輸出List 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="_logList"></param>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public List<ActRollCallLog> CheckLeaveInfo(DateTime dt, List<ActRollCallLog> _logList, int learningId)
        {
            var db = _uow.DbContext;
            var cheLeaveYear = db.AttendanceLeave.Where(t => t.LeaveDate.Year == dt.Year && t.LearningId == learningId);
            var cheLeaveMonth = cheLeaveYear.Where(t => t.LeaveDate.Month == dt.Month);
            var cheLeaveDay = cheLeaveMonth.Where(t => t.LeaveDate.Day == dt.Day);
            foreach (var _item in cheLeaveDay)
            {
                var _entityYear = _logList.Where(t => t.StudId == _item.StudId && t.Created.Utc.Value.Year == _item.LeaveDate.Year);
                var _entityMonth = _entityYear.Where(t => t.StudId == _item.StudId && t.Created.Utc.Value.Month == _item.LeaveDate.Month);
                var _entity = _entityMonth.Where(t => t.StudId == _item.StudId && t.Created.Utc.Value.Day == _item.LeaveDate.Day).FirstOrDefault();
                if (_entity != null)
                {
                    if (_item.Status == "10")
                        _logList[_logList.IndexOf(_entity)].Status = "5";
                }
            }
            return _logList;
        }

        /// <summary>
        /// 取得預設點名紀錄資料(預設狀態:缺席)
        /// </summary>
        /// <param name="creatorId">建立者編號</param>
        /// <param name="memberIds">成員memberId陣列</param>
        private List<ActRollCallLog> GetInitLogs(int creatorId, int[] memberIds)
        {
            // 產生成員預設出缺勤資料
            var logs = new List<ActRollCallLog>();
            foreach (var man in memberIds)
            {
                var log = new ActRollCallLog()
                {
                    StudId = man,
                    Status = AttendanceState.Absence,
                    CreateUser = creatorId,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Updated = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null)
                };
                logs.Add(log);
            }

            return logs;
        }

        /// <summary>
        /// 更新活動持續時間
        /// </summary>
        /// <param name="eventId">活動guid</param>
        /// <param name="memberId">更新者id</param>
        /// <param name="duration">新的活動時間</param>
        /// <returns></returns>
        public DateTime? UpdateDuration(Guid eventId, int memberId, int duration)
        {
            var dt = DateTime.UtcNow;

            #region //1.更新活動長度時間

            var act = _uow.DbContext.Activitys.FirstOrDefault(x => x.OuterKey == eventId);
            if (act == null)
                return null;

            act.Duration = duration;

            //更新活動開始時間
            if (act.StartDate.HasValue)
            {
                var start = DateTime.UtcNow;
                act.StartDate = start.Add(new TimeSpan(0, 0, -duration));
            }

            act.UpdateUser = memberId;
            act.Updated = TimeData.Create(dt);

            #endregion

            #region //2.更新log最後編輯時間
            var signIn = _uow.DbContext.ActRollCall.FirstOrDefault(x => x.EventId == eventId);

            var log = _uow.DbContext.ActRollCallLog.Where(x => x.RollCallId == signIn.Id).ToList();
            log.ForEach(x => { x.UpdateUser = memberId; x.Updated = TimeData.Create(dt); });
            _uow.DbContext.SaveChanges();
            #endregion
            return dt.ToLocalTime();
        }

        /// <summary>
        /// 更新點名開始時間
        /// </summary>
        /// <param name="eventId">活動guid</param>
        /// <param name="memberId">更新者id</param>
        /// <param name="circleId">學習圈編號</param>
        /// <returns></returns>
        public void StartEvent(int circleId, Guid eventId, int memberId)
        {
            // 1.更新開始時間 & 最後編輯時間
            UpdateStartDate(eventId, memberId, true);

            #region // 2.點名名單檢核
            var authService = new AuthService();

            // 1.取得所有應該要被點名的成員
            //  var memberIds = AuthMember.GeFunctionMemberList(circleId, SignInFunction.Member).Select(x => x.AccountId);
            var memberIds = authService.GeFunctionMemberListByLinQ(circleId, SignInFunction.Member).Select(x => x.AccountId);
            var signIn = GetSignInInfomation(eventId);
            var existMembers = signInLogService.GetLogList(signIn.Id);
            // 2.是否有要新增點名的成員
            var extraMembers = memberIds.Except(existMembers.Select(t=>t.StudId)).ToArray();
            var db = _uow.DbContext;
            if (extraMembers.Count() > 0)
            {
                var attendanceRecordService = new AttendanceRecordService();
                // 3.1 整理資料-點名紀錄
                var attendances = attendanceRecordService.GetInitAttendanceRecords(circleId, eventId, extraMembers);
                // 3.2 整理資料-核心出缺勤資料表
                var logs = GetInitLogs(memberId, extraMembers);
                logs.ForEach(x => x.RollCallId = signIn.Id);
                // 3.3 整理資料-查看請假紀錄
                var newLogs = CheckLeaveInfo(signIn.Created.Utc.Value.Date, logs, circleId);
                try
                   {
                            //4.1 新增點名紀錄
                            db.ActRollCallLog.AddRange(newLogs);
                            //4.2 新增核心出缺勤紀錄
                            db.AttendanceRecord.AddRange(attendances);
                            //執行
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
            }
            #endregion

            //// 3.更新log最後編輯時間
            //var log = signInLogService.GetLogList(signIn.Id);
            //log.ForEach(x => { x.UpdateUser = memberId; x.Updated = TimeData.Create(DateTime.UtcNow); });
            //db.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int UpdateStartDate(Guid eventId, int memberId, bool start)
        {
            var db = _uow.DbContext;
            var _aService = new ActivityService();

            var act = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);

            DateTime dt = DateTime.UtcNow;
            //不會被扣除的活動時間 - 最後一次開始時間


            if (start)
            {
                act.StartDate = dt;
                act.ActivityDate = dt;
            }
            else
                act.StartDate = dt.Add(new TimeSpan(0, 0, -act.Duration.Value));

            act.UpdateUser = memberId;
            act.Updated = TimeData.Create(DateTime.UtcNow);

            _uow.SaveChanges();

            return act.Duration.Value;
        }
        /// <summary>
        /// 刪除點名活動
        /// </summary>
        /// <param name="eventId">活動guid</param>
        /// <param name="memberId">更新者id</param>
        /// <returns></returns>
        public void Delete(Guid eventId, int memberId)
        {
            //更新活動刪除時間
            var dt = DateTime.UtcNow;
            var activityService = new ActivityService();


            var act = _uow.DbContext.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (act == null)
                return;
            act.UpdateUser = memberId;
            act.DeleteUser = memberId;
            act.Updated = TimeData.Create(dt);
            act.Deleted = TimeData.Create(dt);
            act.CardisShow = false;
            var signIn = _uow.DbContext.ActRollCall.FirstOrDefault(x => x.EventId == eventId);
            if (signIn == null)
                return;
            signIn.UpdateUser = memberId;
            signIn.DeleteUser = memberId;
            signIn.Updated = TimeData.Create(dt);
            signIn.Deleted = TimeData.Create(dt);

            act.CardisShow = false;
            _uow.SaveChanges();
        }
    }
}

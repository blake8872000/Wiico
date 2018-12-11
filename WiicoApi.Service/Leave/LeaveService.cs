using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Repository;
using WiicoApi.Service.Utility;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService.SignIn;
using WiicoApi.Infrastructure.ViewModel;
using System.IO;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave;

namespace WiicoApi.Service.Leave
{
    public class LeaveService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly SignInLogService signInLogService = new SignInLogService();
        private readonly FileService fileService = new FileService();
        private readonly CacheService cacheService = new CacheService();
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();
        /// <summary>
        /// 存檔Server
        /// </summary>
        private string loginServer = ConfigurationManager.AppSettings["loginServer"].ToString();
        private readonly string systemManager = ConfigurationManager.AppSettings["googleDriveSystemManager"].ToString();

        /// <summary>
        /// 預設縮圖最大寬度
        /// </summary>       
        private readonly int maxImgWidth = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgWidth"]);
        /// <summary>
        /// 預設縮圖最大高度
        /// </summary>
        private readonly int maxImgHeight = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgHeight"]);

        /// <summary>
        /// 為了檔同時多筆請假單使用
        /// </summary>
        public static Object _lock = new Object();

        public LeaveService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 新增請假單 - 預先新增假單
        /// </summary>
        /// <param name="learningId"></param>
        /// <param name="isTeacher"></param>
        /// <param name="memberId"></param>
        /// <param name="leaveDateTime"></param>
        /// <param name="leaveCategoryId"></param>
        /// <param name="leaveStatus"></param>
        /// <param name="title"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.SignInLogViewModel Add(LearningCircle learningInfo, int memberId, Infrastructure.DataTransferObject.UploadFileModel uploadInfo, List<FileStorage> files, List<Stream> fileStreams)
        {
            var parentFolderId = string.Empty;

            var db = _uow.DbContext;
            var connectionInfo = db.Members.Find(memberId);
            var memberAccount = _uow.EntityRepository<Member>().GetFirst(x => x.Id == memberId);
            if (memberAccount == null)
                return null;

            var _entity = new AttendanceLeave();
            var _actEntity = new Activitys();

            lock (_lock)
            {
                DateTime dt = DateTime.UtcNow;
                Guid eveId = Guid.NewGuid();
                try
                {
                    var studentLeave = db.AttendanceLeave.Where(t => t.StudId == memberId).ToList();
                    var checkIsLeave = new AttendanceLeave();
                    if (studentLeave.FirstOrDefault() != null)
                        checkIsLeave = studentLeave.FirstOrDefault(t => t.LeaveDate.Date == uploadInfo.LeaveDate.Date && t.Status != "40" && t.LearningId == learningInfo.Id);
                    else
                        checkIsLeave = null;
                    if (checkIsLeave == null)
                    {
                        _entity.Content = uploadInfo.Content;
                        _entity.CreateDate = dt;
                        _entity.Creator = memberId;
                        _entity.EventId = eveId;
                        _entity.LearningId = learningInfo.Id;
                        if (uploadInfo.LeaveDate.Hour == 0)
                            _entity.LeaveDate = uploadInfo.LeaveDate;
                        else
                            _entity.LeaveDate = uploadInfo.LeaveDate.ToUniversalTime();
                        _entity.LeaveType = uploadInfo.LeaveCategoryId;
                        _entity.Status = QueryCondition.LeaveState.Review;
                        _entity.StudId = memberId;
                        _entity.Subject = uploadInfo.Title;
                        db.AttendanceLeave.Add(_entity);
                        _actEntity.Created = Infrastructure.Property.TimeData.Create(dt);
                        _actEntity.Updated = Infrastructure.Property.TimeData.Create(null);
                        _actEntity.Deleted = Infrastructure.Property.TimeData.Create(null);
                        _actEntity.CreateUser = memberId;
                        _actEntity.IsActivity = false;
                        _actEntity.ModuleKey = Utility.ParaCondition.ModuleType.Leave;
                        _actEntity.OuterKey = eveId;
                        _actEntity.CardisShow = false;
                        _actEntity.Publish_Utc = dt;
                        _actEntity.ToRoomId = learningInfo.LearningOuterKey;
                        db.Activitys.Add(_actEntity);

                        //執行
                        db.SaveChanges();


                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {

                    return null;
                    throw ex;
                }

            }


            var newFiles = fileService.UploadFiles(memberId, files, fileStreams.ToArray());
            var _fileArray = new string[files.Count];
            if (files.Count > 0)
                //建立檔案與請假單的關聯
                SetLeaveFileReferenceByFiles(_entity.Id, newFiles);

            var result = new Infrastructure.ViewModel.SignInLogViewModel
            {
                EventId = _entity.EventId,
                OuterKeySignInLog = new Dictionary<string, SignInLog>()
            };
            //db = new iThinkDB();
            var sqlSignInActivity = db.Activitys.Where(t => t.ToRoomId.ToLower() == uploadInfo.CircleKey.ToLower() && t.ModuleKey == Utility.ParaCondition.ModuleType.SignIn && t.CardisShow == true && t.StartDate.HasValue);
            foreach (var _act in sqlSignInActivity)
            {
                if (_act.ActivityDate != null)
                {
                    if (_act.ActivityDate.Value.ToLocalTime().Date == uploadInfo.LeaveDate.Date)
                    {
                        var mylog = signInLogService.GetSignInLog(Convert.ToInt32(_act.CreateUser.ToString()), _act.OuterKey, memberId);
                        if (mylog != null)
                        {
                            mylog.LeaveStatus = "20";
                            mylog.Mode = 0;
                            mylog.Time = DateTime.UtcNow;
                            var outerKey = Utility.OuterKeyHelper.GuidToPageToken(_act.OuterKey);
                            result.OuterKeySignInLog.Add(outerKey, mylog);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 取得請假單資訊 - forAPP
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="isManage"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.ActivityFunction.Leave.GetAbSenceFormDetailResponse GetAbSenceFormDetail(Infrastructure.ViewModel.ActivityFunction.Leave.GetAbSenceFormDetailRequest requestData)
        {

            var eventId = Utility.OuterKeyHelper.CheckOuterKey(requestData.OuterKey);
            if (eventId == null || eventId == Guid.Empty)
                return null;
            var authService = new AuthService();
            var learningcircleService = new LearningCircleService();
            var learningInfo = learningcircleService.GetDetailByOuterKey(requestData.ClassID);
            if (learningInfo == null)
                return null;
            var tokenService = new TokenService();
            var checkMember = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkMember == null)
                return null;
            var isManage = authService.CheckFunctionAuth(learningInfo.Id, ParaCondition.LeaveFunction.Review, checkMember.MemberId);

            return GetAPPDetailLeaveSqlQuery(_uow.DbContext.AttendanceLeave.FirstOrDefault(t => t.EventId == eventId), isManage);
        }

        /// <summary>
        /// 取得單一請假單詳細資訊 - for app
        /// </summary>
        /// <param name="sqlLeave"></param>
        /// <param name="isManage"></param>
        /// <returns></returns>
        private Infrastructure.ViewModel.ActivityFunction.Leave.GetAbSenceFormDetailResponse GetAPPDetailLeaveSqlQuery(AttendanceLeave sqlLeave, bool isManage)
        {
            var db = _uow.DbContext;
            var resLeaveVM = new Infrastructure.ViewModel.ActivityFunction.Leave.GetAbSenceFormDetailResponse();
            resLeaveVM.FileList = new List<FileStorageViewModel>();

            //取得請假資訊
            resLeaveVM.Comment = sqlLeave.Comment;
            resLeaveVM.CreateDate = sqlLeave.CreateDate.ToLocalTime();
            resLeaveVM.Content = sqlLeave.Content;
            resLeaveVM.LeaveDate = sqlLeave.LeaveDate.ToLocalTime();
            resLeaveVM.Subject = sqlLeave.Subject;
            //取得狀態名稱
            // resLeaveVM.StatusName = Utility.ParaCondition.LeaveState.Status[sqlLeave.Status];
            //申請人資訊
            var applicationMemberInfo = db.Members.Find(sqlLeave.StudId);
            //請假單申請人照片
            resLeaveVM.MemberImg = applicationMemberInfo.Photo;
            //請假單申請人姓名
            resLeaveVM.MemberAccount = applicationMemberInfo.Account;
            //請假單申請人帳號
            resLeaveVM.MemberName = applicationMemberInfo.Name;
            var learningCircleInfo = db.LearningCircle.Find(sqlLeave.LearningId);
            //所屬課程名稱
            resLeaveVM.CourseName = learningCircleInfo.Name;
            //編碼eventId
            resLeaveVM.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(sqlLeave.EventId);

            var timeTableData = db.TimeTable.FirstOrDefault(t => t.Course_No == learningCircleInfo.LearningOuterKey && t.StartDate >= sqlLeave.LeaveDate);
            if (timeTableData == null || (timeTableData.StartDate.Value.DayOfWeek != sqlLeave.LeaveDate.DayOfWeek))
                timeTableData = null;

            //上課地點與時間
            resLeaveVM.Rooms = new List<Infrastructure.ViewModel.ActivityFunction.Leave.ClassRoomInfo>();
            var sectionDateTimeDataService = new Utility.DateTimeTools();
            var dayofweekService = new DayOfWeekTools();
            if (timeTableData != null)
            {
                var weekstring = dayofweekService.ChangeToCht(sqlLeave.LeaveDate.DayOfWeek);
                var weekTableData = db.WeekTable.FirstOrDefault(t => t.LearningCircleId == sqlLeave.LearningId && t.Week == weekstring);
                if (weekTableData != null)
                {
                    for (var i = weekTableData.StartPeriod.Value; i <= weekTableData.EndPeriod.Value; i++)
                    {
                        var data = new Infrastructure.ViewModel.ActivityFunction.Leave.ClassRoomInfo()
                        {
                            classStart = sectionDateTimeDataService.ProxyDateTime(timeTableData.StartDate.Value.Date, i, true),
                            ClassEnd = sectionDateTimeDataService.ProxyDateTime(timeTableData.StartDate.Value.Date, i, false),
                            RoomId = timeTableData.ClassRoomId,
                            RoomName = timeTableData.ClassRoom,
                            NameOfWeekDay = weekstring
                        };
                        resLeaveVM.Rooms.Add(data);
                    }
                }
            }
            resLeaveVM.Status = sqlLeave.Status;
            /* //設定狀態
             switch (sqlLeave.Status)
             {
                 case "00":
                     resLeaveVM.Status = enumAbsenceFormStatus.Invalid;
                     break;
                 case "10":
                     resLeaveVM.Status = enumAbsenceFormStatus.Pass;
                     break;
                 case "20":
                     resLeaveVM.Status = enumAbsenceFormStatus.Wait;
                     break;
                 case "30":
                     resLeaveVM.Status = enumAbsenceFormStatus.Recall;
                     break;
                 case "40":
                     resLeaveVM.Status = enumAbsenceFormStatus.Reject;
                     break;
                 default:
                     break;
             }*/
            if (sqlLeave.LeaveType.HasValue)
            {
                //設定假單類型
                switch (sqlLeave.LeaveType.Value)
                {
                    case 1:
                        resLeaveVM.LeaveType = enumLeaveType.SickLeave;
                        break;
                    case 2:
                        resLeaveVM.LeaveType = enumLeaveType.PersonalLeave;
                        break;
                    case 3:
                        resLeaveVM.LeaveType = enumLeaveType.LeaveForStatutory;
                        break;
                    case 4:
                        resLeaveVM.LeaveType = enumLeaveType.Other;
                        break;
                    default:
                        break;
                }
            }

            //取得請假單檔案資訊
            var sqlFile = db.LeaveFile.Where(t => t.LeaveId == sqlLeave.Id);
            foreach (var _file in sqlFile)
            {
                var _fileEntity = new FileStorageViewModel();
                var googleFile = db.FileStorage.Find(_file.GoogleId);
                //if (googleFile == null)
                //    _fileEntity = db.GoogleFile.Find(_file.GoogleId);
                //else
                //{
                //萬一與googleFileId一樣，判斷假單建立日期與檔案建立日期是否一致
                //var checkCreateDate = googleFile.CreateUtcDate.Date;
                //if (checkCreateDate == sqlLeave.CreateDate.Date)
                //{
                //    _fileEntity = new FileViewModel()
                //    {
                //        Create_User = googleFile.Creator,
                //        Create_Utc = googleFile.CreateUtcDate.ToLocalTime(),
                //        DownLoadUrl = googleFile.FileUrl,
                //        FileId = googleFile.FileGuid.ToString("N"),
                //        FileType = googleFile.FileContentType,
                //        Id = googleFile.Id,
                //        Name = googleFile.Name,
                //        Size = googleFile.FileSize,
                //        WebViewUrl = googleFile.FileUrl
                //    };
                //    if (googleFile.FileContentType.ToString().StartsWith("image"))
                //        _fileEntity.ImgUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, googleFile.FileGuid.ToString("N"), maxImgWidth, maxImgHeight);
                //}
                //else
                //    _fileEntity = db.GoogleFile.Find(_file.GoogleId);
                //}

                if (googleFile.FileContentType.ToString().StartsWith("image"))
                {
                    _fileEntity.FileImageUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, googleFile.FileGuid.ToString("N"), maxImgWidth, maxImgHeight);
                    _fileEntity.ImgUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, googleFile.FileGuid.ToString("N"), maxImgWidth, maxImgHeight);
                }
                _fileEntity.WebViewUrl = _fileEntity.FileUrl;
                _fileEntity.DownLoadUrl = _fileEntity.FileUrl;
                _fileEntity.Size = _fileEntity.FileSize;
                _fileEntity.FileType = _fileEntity.FileContentType;
                if (_fileEntity != null)
                    resLeaveVM.FileList.Add(_fileEntity);
            }
            resLeaveVM.IsManager = isManage;


            return resLeaveVM;
        }


        /// <summary>
        /// 取得列表 - ForAPP
        /// </summary>
        /// <returns></returns>
        public Infrastructure.ViewModel.ActivityFunction.Leave.GetAbsenceFormListResponse GetAbsenceFormList(Infrastructure.ViewModel.ActivityFunction.Leave.GetAbsenceFormListRequest requestData)
        {
            var tokenService = new TokenService();
            var checkMember = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkMember == null)
                return null;
            var authService = new AuthService();
            var learningcircleService = new LearningCircleService();
            var learningInfo = learningcircleService.GetDetailByOuterKey(requestData.ClassID);
            if (learningInfo == null)
                return null;
            var auth = authService.CheckFunctionAuth(learningInfo.Id, ParaCondition.LeaveFunction.Review, checkMember.MemberId);
            var responseData = new Infrastructure.ViewModel.ActivityFunction.Leave.GetAbsenceFormListResponse();
            if (auth)
            {
                //老師介面
                var searchData = requestData.CheckDate.HasValue ?
                    GetAppList(learningInfo.Id, true, null, requestData.CheckDate.Value) :
                    GetAppList(learningInfo.Id, true, null, null);
                responseData.Forms = searchData;
                responseData.IsQueryFormSuccess = true;
                responseData.IsManager = true;
                responseData.IsQueryRoomSuccess = true;
            }
            else
            {
                //學生介面
                var searchData = requestData.CheckDate.HasValue ?
                    GetAppList(learningInfo.Id, false, checkMember.MemberId, requestData.CheckDate.Value) :
                    GetAppList(learningInfo.Id, false, checkMember.MemberId, null);
                responseData.Forms = searchData;
                responseData.IsQueryFormSuccess = true;
                responseData.IsManager = false;
                responseData.IsQueryRoomSuccess = true;
            }
            responseData.ClassID = learningInfo.LearningOuterKey;
            responseData.ClassName = learningInfo.Name;
            return responseData;
        }

        /// <summary>
        /// 建立請假單
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.SignInLogViewModel SetNewAbsenceForm(Infrastructure.ViewModel.ActivityFunction.Leave.SetNewAbsenceFormRequest requestData, List<FileStorage> files, List<Stream> streams)
        {
            var tokenService = new TokenService();
            var checkMember = tokenService.GetTokenInfo(requestData.Token.ToString()).Result;
            if (checkMember == null)
                return null;
            var authService = new AuthService();
            var learningcircleService = new LearningCircleService();
            var learningInfo = learningcircleService.GetDetailByOuterKey(requestData.CircleKey);
            if (learningInfo == null)
                return null;
            var auth = authService.CheckFunctionAuth(learningInfo.Id, ParaCondition.LeaveFunction.Create, checkMember.MemberId);

            if (auth)
            {

                var uploadData = new Infrastructure.DataTransferObject.UploadFileModel()
                {
                    Token = requestData.Token,
                    Title = requestData.Title,
                    Content = requestData.Content,
                    CircleKey = requestData.CircleKey,
                    LeaveDate = requestData.LeaveDate,
                    LeaveCategoryId = requestData.LeaveCategoryId
                };
                var createRespone = Add(learningInfo, checkMember.MemberId, uploadData, files, streams);
                if (createRespone == null)
                    return null;
                else
                    return createRespone;
            }
            return null;
        }

        /// <summary>
        /// 取得請假單資訊
        /// </summary>
        /// <param name="learningId"></param>
        /// <param name="isTeacher"></param>
        /// <param name="memberId"></param>
        /// <param name="leaveDateTime"></param>
        /// <returns></returns>
        public List<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveData> GetAppList(int learningId, bool isTeacher, int? memberId, DateTime? leaveDateTime)
        {
            var db = _uow.DbContext;
            var sqlLeave = (memberId.HasValue && isTeacher == false) ?
                db.AttendanceLeave.Where(t => t.StudId == memberId && t.LearningId == learningId) :
                 // _uow.EntityRepository<AttendanceLeave>().Get(t => t.StudId == memberId && t.LearningId == learningId) :
                 db.AttendanceLeave.Where(t => t.LearningId == learningId);

            if (leaveDateTime.HasValue)
            {
                var compareYear = sqlLeave.Where(t => t.LeaveDate.Year == leaveDateTime.Value.Year &&
                                                                                             t.LeaveDate.Month == leaveDateTime.Value.Month &&
                                                                                             t.LeaveDate.Day == leaveDateTime.Value.Day).ToList();
                /*    var compareMonth = compareYear.Where(t => t.LeaveDate.Month == leaveDateTime.Value.Month);
                    var compareDay = compareMonth.Where(t => t.LeaveDate.Day == leaveDateTime.Value.Day);*/
                return GetAPPLeaveSqlQuery(compareYear, isTeacher);
            }
            else
                return GetAPPLeaveSqlQuery(sqlLeave.ToList(), isTeacher);
        }

        /// <summary>
        /// 取得請假單資訊
        /// </summary>
        /// <param name="learningId"></param>
        /// <param name="isTeacher">是否為老師</param>
        /// <param name="memberId">建立者</param>
        /// <param name="leaveDateTime">請假日期</param>
        /// <param name="leaveCategoryId">假單類別</param>
        /// <param name="leaveStatus">假單狀態</param>
        /// <returns></returns>
        public List<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveViewModel> Get(int learningId, bool isTeacher, int? memberId, DateTime? leaveDateTime)
        {
            var db = _uow.DbContext;
            var sqlLeave = (memberId.HasValue && isTeacher == false) ?
                _uow.EntityRepository<AttendanceLeave>().Get(t => t.StudId == memberId && t.LearningId == learningId) :
                _uow.EntityRepository<AttendanceLeave>().Get(t => t.LearningId == learningId);
            if (leaveDateTime.HasValue)
            {
                var compareYear = sqlLeave.Where(t => t.LeaveDate.Year == leaveDateTime.Value.Year);
                var compareMonth = compareYear.Where(t => t.LeaveDate.Month == leaveDateTime.Value.Month);
                var compareDay = compareMonth.Where(t => t.LeaveDate.Day == leaveDateTime.Value.Day);
                return GetLeaveSqlQuery(compareDay, isTeacher);
            }
            else
                return GetLeaveSqlQuery(sqlLeave, isTeacher);
        }
        /// <summary>
        /// 取得請假單資訊
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="isManage"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.ActivityFunction.Leave.LeaveDetailViewModel GetDetail(Guid? eventId, bool isManage)
        {
            var leaveData = _uow.DbContext.AttendanceLeave.FirstOrDefault(t => t.EventId == eventId);
            if (leaveData == null)
                return null;
            return GetDetailLeaveSqlQuery(leaveData, isManage);
        }


        /// <summary>
        /// 變更請假單狀態/資訊 - 審查狀態
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="leaveDateTime"></param>
        /// <param name="leaveCategoryId"></param>
        /// <param name="leavereason"></param>
        /// <param name="leaveStatus"></param>
        /// <returns></returns>
        public AttendanceLeave Update(Guid eventId, DateTime? leaveDateTime, int? leaveCategoryId, string leavereason, string leaveStatus)
        {
            var dt = DateTime.UtcNow;
            var db = _uow.DbContext;
            try
            {
                var _entity = db.AttendanceLeave.Where(t => t.EventId == eventId).FirstOrDefault();

                if (leavereason != string.Empty && leavereason != "")
                    _entity.Comment = leavereason;
                if (leaveDateTime != null)
                    _entity.LeaveDate = leaveDateTime.Value.ToUniversalTime();
                if (leaveCategoryId != null)
                    _entity.LeaveType = leaveCategoryId;
                if (leaveStatus != string.Empty && leaveStatus != "")
                    _entity.Status = leaveStatus;
                _entity.UpdateTime = dt;
                //執行
                db.SaveChanges();
                return _entity;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }


        /// <summary>
        /// 查詢請假資訊 - forapp
        /// </summary>
        /// <param name="sqlLeave"></param>
        /// <param name="isManager"></param>
        /// <returns></returns>
        private List<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveData> GetAPPLeaveSqlQuery(List<AttendanceLeave> sqlLeave, bool isManager)
        {

            var result = new List<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveData>();
            var db = _uow.DbContext;
            var list = sqlLeave.OrderByDescending(t => t.Id).ToList();
            var responseData = from data in list
                               join studentData in db.Members on data.StudId equals studentData.Id
                               select new Infrastructure.ViewModel.ActivityFunction.Leave.LeaveData
                               {
                                   Comment = data.Comment,
                                   CreateDate = data.CreateDate.ToLocalTime(),
                                   Content = data.Content,
                                   LeaveDate = data.LeaveDate.ToLocalTime(),
                                   Subject = data.Subject,
                                   MemberAccount = studentData.Account,
                                   MemberName = studentData.Name,
                                   Status = data.Status,
                                   OuterKey = OuterKeyHelper.GuidToPageToken(data.EventId),
                                   LeaveType = data.LeaveType.HasValue ? (enumLeaveType)data.LeaveType.Value : 0,
                                   IsManager = isManager,
                                   LeaveId = data.Id,
                                   FileList = 
                                   //db.LeaveFile.FirstOrDefault(t => t.LeaveId == data.Id) != null ?
                                   //             (from fs in db.FileStorage
                                   //            join lf in db.LeaveFile on fs.Id equals lf.GoogleId
                                   //            where lf.LeaveId == data.Id
                                   //            select new FileStorageViewModel
                                   //            {
                                   //                CreateUtcDate = fs.CreateUtcDate,
                                   //                FileSize = fs.FileSize,
                                   //                Creator = fs.Creator,
                                   //                Deleter = fs.Deleter,
                                   //                DeleteUtcDate = fs.DeleteUtcDate,
                                   //                DownLoadUrl = fs.FileUrl,
                                   //                FileContentType = fs.FileContentType,
                                   //                FileGuid = fs.FileGuid,
                                   //                FileImageHeight = fs.FileImageHeight,
                                   //                FileImageWidth = fs.FileImageWidth,
                                   //                FileType = fs.FileContentType,
                                   //                FileUrl = fs.FileUrl,
                                   //                Id = fs.Id,
                                   //                Name = fs.Name,
                                   //                Size = fs.FileSize,
                                   //                WebViewUrl = fs.FileUrl,
                                   //                //  ImgUrl = loginServer+ "api/imgFile/"+ fs.FileGuid+"/"+ maxImgWidth+"/"+ maxImgHeight ,
                                   //                ImgUrl = fs.FileUrl,
                                   //                FileImageUrl = fs.FileUrl
                                   //            }).ToList() :
                                               new List<FileStorageViewModel>()
                               };
            return responseData.FirstOrDefault() != null ? responseData.ToList() : result;
        }

        /// <summary>
        /// 取得單一請假單詳細資訊
        /// </summary>
        /// <param name="sqlLeave"></param>
        /// <param name="isManage"></param>
        /// <returns></returns>
        private Infrastructure.ViewModel.ActivityFunction.Leave.LeaveDetailViewModel GetDetailLeaveSqlQuery(AttendanceLeave sqlLeave, bool isManage)
        {
            var db = _uow.DbContext;
            var resLeaveVM = new Infrastructure.ViewModel.ActivityFunction.Leave.LeaveDetailViewModel();
            resLeaveVM.FileList = new List<GoogleFile>();

            //取得請假資訊
            resLeaveVM.Comment = sqlLeave.Comment;
            resLeaveVM.CreateDate = sqlLeave.CreateDate.ToLocalTime();
            resLeaveVM.Content = sqlLeave.Content;
            resLeaveVM.LeaveDate = sqlLeave.LeaveDate.ToLocalTime();
            resLeaveVM.LeaveType = sqlLeave.LeaveType;
            resLeaveVM.Status = sqlLeave.Status;
            resLeaveVM.StudId = sqlLeave.StudId;
            resLeaveVM.Subject = sqlLeave.Subject;
            //取得狀態名稱
            resLeaveVM.StatusName = Utility.ParaCondition.LeaveState.Status[sqlLeave.Status];
            //申請人資訊
            var applicationMemberInfo = db.Members.Find(sqlLeave.StudId);

            //請假單申請人姓名
            resLeaveVM.MemberAccount = applicationMemberInfo.Account;
            //請假單申請人帳號
            resLeaveVM.MemberName = applicationMemberInfo.Name;
            //請假單申請人頭像
            resLeaveVM.MemberImg = applicationMemberInfo.Photo;
            //所屬課程名稱
            resLeaveVM.CourseName = db.LearningCircle.Find(sqlLeave.LearningId).Name;
            //編碼eventId
            resLeaveVM.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(sqlLeave.EventId);

            //取得請假單檔案資訊
            var sqlFile = db.LeaveFile.Where(t => t.LeaveId == sqlLeave.Id);
            foreach (var _file in sqlFile)
            {
                var googleFile = db.GoogleFile.Find(_file.GoogleId);

                var _fileEntity = new GoogleFile()
                {
                    Create_User = googleFile.Create_User,
                    Create_Utc = googleFile.Create_Utc.ToLocalTime(),
                    DownLoadUrl = googleFile.DownLoadUrl,
                    FileId = googleFile.FileId,
                    FileType = googleFile.FileType,
                    Id = googleFile.Id,
                    ImgUrl = googleFile.ImgUrl,
                    Name = googleFile.Name,
                    ParentFileId = googleFile.ParentFileId,
                    Size = googleFile.Size,
                    WebViewUrl = googleFile.WebViewUrl
                };

                resLeaveVM.FileList.Add(_fileEntity);

            }
            resLeaveVM.IsManager = isManage;
            return resLeaveVM;
        }

        /// <summary>
        /// 建立請假單與檔案的關聯
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="files"></param>
        private void SetLeaveFileReferenceByFiles(int leaveId, List<FileStorageViewModel> files)
        {

            var db = _uow.DbContext;
            try
            {
                foreach (var file in files)
                {
                    var leaveFileEntity = new LeaveFile();
                    leaveFileEntity.LeaveId = leaveId;
                    leaveFileEntity.GoogleId = file.Id;
                    db.LeaveFile.Add(leaveFileEntity);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 建立檔案與請假單的關聯
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="googleDriveFileId"></param>
        /// <returns></returns>
        private void SetLeaveFileReference(int leaveId, string[] googleDriveFileId)
        {
            var result = new LeaveFile();
            using (var db = _uow.DbContext)
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var _item in googleDriveFileId)
                        {
                            var _entity = new LeaveFile();
                            _entity.LeaveId = leaveId;
                            var _file = db.GoogleFile.Where(t => t.FileId == _item.ToString()).FirstOrDefault();
                            _entity.GoogleId = _file.Id;
                            db.LeaveFile.Add(_entity);

                        }

                        //執行
                        db.SaveChanges();
                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        throw ex;
                    }
                }
            }
        }



        /// <summary>
        /// 查詢請假資訊
        /// </summary>
        /// <param name="sqlLeave"></param>
        /// <param name="isManager"></param>
        /// <returns></returns>
        private List<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveViewModel> GetLeaveSqlQuery(IQueryable<AttendanceLeave> sqlLeave, bool isManager)
        {
            var result = new List<Infrastructure.ViewModel.ActivityFunction.Leave.LeaveViewModel>();
            var db = _uow.DbContext;
            foreach (var _item in sqlLeave.OrderByDescending(t => t.Id))
            {
                var resLeaveVM = new Infrastructure.ViewModel.ActivityFunction.Leave.LeaveViewModel();
                var learningInfo = db.LearningCircle.Find(_item.LearningId);
                resLeaveVM.CourseName = learningInfo.Name;
                resLeaveVM.FileList = new List<GoogleFile>();
                //取得請假資訊
                resLeaveVM.Info = new AttendanceLeave()
                {
                    Comment = _item.Comment,
                    CreateDate = _item.CreateDate.ToLocalTime(),
                    EventId = _item.EventId,
                    Content = _item.Content,
                    Creator = _item.Creator,
                    Id = _item.Id,
                    LearningId = _item.LearningId,
                    LeaveDate = _item.LeaveDate.ToLocalTime(),
                    LeaveType = _item.LeaveType,
                    Status = _item.Status,
                    StudId = _item.StudId,
                    Subject = _item.Subject
                };
                if (_item.UpdateTime != null)
                    resLeaveVM.Info.UpdateTime = _item.UpdateTime.Value.ToLocalTime();

                var applicationMemberInfo = db.Members.Find(_item.StudId);
                //請假單申請人姓名
                resLeaveVM.MemberAccount = applicationMemberInfo.Account;
                //請假單申請人帳號
                resLeaveVM.MemberName = applicationMemberInfo.Name;
                //編碼eventId
                resLeaveVM.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(_item.EventId);
                //取得狀態名稱
                resLeaveVM.StatusName = Utility.ParaCondition.LeaveState.Status[_item.Status];
                //取得請假單檔案資訊
                var sqlFile = db.LeaveFile.Where(t => t.LeaveId == _item.Id);
                foreach (var _file in sqlFile)
                {
                    var googleFile = db.GoogleFile.Find(_file.GoogleId);
                    var _fileEntity = new GoogleFile()
                    {
                        Create_User = googleFile.Create_User,
                        Create_Utc = googleFile.Create_Utc.ToLocalTime(),
                        DownLoadUrl = googleFile.DownLoadUrl,
                        FileId = googleFile.FileId,
                        FileType = googleFile.FileType,
                        Id = googleFile.Id,
                        ImgUrl = googleFile.ImgUrl,
                        Name = googleFile.Name,
                        ParentFileId = googleFile.ParentFileId,
                        Size = googleFile.Size,
                        WebViewUrl = googleFile.WebViewUrl
                    };

                    resLeaveVM.FileList.Add(_fileEntity);
                }
                resLeaveVM.IsManager = isManager;
                result.Add(resLeaveVM);
            }
            return result;
        }
    }
}

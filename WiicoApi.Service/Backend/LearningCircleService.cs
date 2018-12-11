using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.BusinessObject;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Repository;
using WiicoApi.Service.Backend;
using WiicoApi.Service.SignalRService;

namespace WiicoApi.Service.CommenService
{
    public class LearningCircleService
    {
        private readonly GenericUnitOfWork _uow;
        private static LearningCircleService _learningcircleService;
        private readonly AuthService authService = new AuthService();
        private readonly LearningRoleService learningRoleService = new LearningRoleService();

        public LearningCircleService()
        {
            _uow = new GenericUnitOfWork();
        }
        public LearningCircleService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }
        public LearningCircleService(string connecitonName)
        {
            _uow = new GenericUnitOfWork(connecitonName);
        }
        /// <summary>
        /// 根據學習圈代碼取得學習圈詳細資料
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public LearningCircle GetDetailByOuterKey(string outerKey)
        {
            var entity = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == outerKey && t.Enable == true && t.Visibility == true);
            return entity;
        }

        /// <summary>
        /// 根據學習圈編號取得學習圈詳細資料
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public LearningCircle GetDetailByLearningCircleId(int learningCircleId)
        {
            var entity = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.Id == learningCircleId && t.Enable == true && t.Visibility == true);
            return entity;
        }

        /// <summary>
        /// 根據使用者編號取得課程列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<LearningCircle>GetListByMemberId(int memberId) {
            return _uow.LearningCircleRepo.GetListByMemberId(memberId);
        }


        /// <summary>
        /// 根據使用者編號取得課程列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<LearningCircle> GetListByUserToken(string token)
        {
            return _uow.LearningCircleRepo.GetListByUserToken(token);
        }

        /// <summary>
        /// 取得課程詳細資訊 - 新的雲端版web專用
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Backend.LearningCircleGetResponse GetLearningCircleInfo(string circleKey)
        {
            var db = _uow.DbContext;
            var leanringCircleInfo = GetDetailByOuterKey(circleKey.ToLower());
            if (leanringCircleInfo == null)
                return null;
            var responseData = new Infrastructure.ViewModel.Backend.LearningCircleGetResponse()
            {
                CircleKey = circleKey,
                Description = leanringCircleInfo.Description,
                Name = leanringCircleInfo.Name,
                EndDate = leanringCircleInfo.EndDate.Value.ToLocalTime(),
                StartDate = leanringCircleInfo.StartDate.Value.ToLocalTime(),
                Objective = leanringCircleInfo.Objective,
                Remark = leanringCircleInfo.ReMark,
                DomainId = leanringCircleInfo.ModuleName,
                DomainName = leanringCircleInfo.ModuleName
            };
            var memberService = new MemberService();
            //處理老師名單
            var teachers = memberService.GetTeacherList(circleKey.ToLower());
            if (teachers.FirstOrDefault() != null)
            {
                var teachersName = teachers.Select(t => t.Name).ToList();
                responseData.Teachers = new List<string>();
                foreach (var teacher in teachersName)
                {
                    responseData.Teachers.Add(teacher);
                }
            }

            var weekTableService = new WeekTableService();
            var ClassWeekTableDatas = weekTableService.GetByCirclekey(circleKey);
            if (ClassWeekTableDatas.WeekTable.FirstOrDefault() != null)
            {
                responseData.WeekTable = new List<Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable>();
                foreach (var weekTableData in ClassWeekTableDatas.WeekTable)
                {
                    var data = new Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable()
                    {
                        Week = weekTableData.Week,
                        Place = weekTableData.Place,
                        StartPeriod = weekTableData.StartPeriod.Value,
                        EndPeriod = weekTableData.EndPeriod.Value,
                        StartTime = string.Format("{0:HH}:{0:mm}", weekTableData.StartTime.ToLocalTime()),
                        EndTime = string.Format("{0:HH}:{0:mm}", weekTableData.EndTime.ToLocalTime())
                    };
                    responseData.WeekTable.Add(data);
                }
            }
            return responseData;
        }

        /// <summary>
        /// 取得新增學習圈的頁面框架資訊
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Backend.LearningCircleViewModel GetCreateLearningCircleViewModelInformation(int orgId)
        {
            var backendService = new BackendService();
            var result = new Infrastructure.ViewModel.Backend.LearningCircleViewModel();
            var extensionColumns = backendService.GetExtensionColumns(orgId);
            result.ExtensionInfo = new List<Infrastructure.ViewModel.Backend.LearningCircleExt>();
            foreach (var column in extensionColumns)
            {
                var resExtInfo = new Infrastructure.ViewModel.Backend.LearningCircleExt();
                resExtInfo.Column = column;
                result.ExtensionInfo.Add(resExtInfo);
            }
            return result;
        }

        /// <summary>
        /// 取得編輯學習圈的頁面框架資訊
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Backend.LearningCircleViewModel GetUpdateLearningCircleViewModelInformationBycirclekey(string circlekey, int orgId)
        {
            var learningCircleInfo = GetDetailByOuterKey(circlekey);
            if (learningCircleInfo == null)
                return null;

            return GetUpdateLearningCircleViewModelInformation(learningCircleInfo.Id, orgId);
        }

        /// <summary>
        /// 取得編輯學習圈的頁面框架資訊
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Backend.LearningCircleViewModel GetUpdateLearningCircleViewModelInformation(int learningCircleId, int orgId)
        {
            var db = _uow.DbContext;
            var backendService = new Service.Backend.BackendService();
            var result = new Infrastructure.ViewModel.Backend.LearningCircleViewModel();
            var learningCircleInfo = GetDetailByLearningCircleId(learningCircleId);
            if (learningCircleInfo == null)
                return null;

            result.LearningInfo = learningCircleInfo;

            var extensionColumns = backendService.GetExtensionColumns(orgId);
            result.ExtensionInfo = new List<Infrastructure.ViewModel.Backend.LearningCircleExt>();
            foreach (var column in extensionColumns)
            {
                var resExtInfo = new Infrastructure.ViewModel.Backend.LearningCircleExt();
                var resExtValue = db.LCExtensionValue.FirstOrDefault(t => t.ColumnId == column.Id && t.DataId == learningCircleId);
                resExtInfo.Column = column;
                resExtInfo.Value = resExtValue;
                result.ExtensionInfo.Add(resExtInfo);
            }

            return result;
        }

        /// <summary>
        /// 取得後臺介面使用者所管理的學習圈列表
        /// </summary>
        /// <param name="memberId">登入者編號</param>
        ///  <param name="searchString">查詢名稱條件</param>
        /// <param name="deptId">選擇要查詢的學院資料</param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.Entity.LearningCircle> GetBackendLearningCircleManageList(int memberId, string searchString, int? deptId = null)
        {
            var db = _uow.DbContext;

            //是否為素人
            var checkIsAmateur = (from o in db.Organizations
                                  join m in db.Members on o.Id equals m.OrgId
                                  where o.OrgCode == "amateur" && m.Id == memberId && m.RoleName == "2"
                                  select m).FirstOrDefault();


            var query = (checkIsAmateur != null)
                ?  //是素人老師
                    (db.LearningCircle.Where(t => t.CreateUser == memberId && t.Enable == true && t.Visibility == true).ToList())
                  : //是有學校組織 或 系統管理者
                    //有查詢學院條件
                        (deptId.HasValue)
                              ? ((from lc in db.LearningCircle
                                  join c in db.Courses on lc.CourseId equals c.Id
                                  where //是否為某個學院的編號
                                              c.DeptId == deptId.Value && lc.Enable == true && lc.Visibility == true
                                  select lc).ToList())
                              //沒有查詢學院條件
                              : ((from lc in db.LearningCircle
                                      //      join c in db.Courses on lc.CourseId equals c.Id
                                      //      join d in db.Depts on c.DeptId equals d.Id
                                  join o in db.Organizations on lc.OrgId equals o.Id
                                  join m in db.Members on o.Id equals m.OrgId
                                  where m.Id == memberId && lc.Enable == true && lc.Visibility == true
                                  select lc).ToList());
            return (searchString != string.Empty && searchString != null) ? query.Where(t => t.Name.Contains(searchString) || t.LearningOuterKey.Contains(searchString)).OrderBy(t => t.Id).ToList() : query.OrderBy(t => t.Id).ToList();
        }

        /// <summary>
        /// 取得管理者學習圈列表資訊
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="searchString"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.Backend.LearningCircleManageViewModel> GetLearningCircleManageList(int memberId, string searchString, int orgId, int? deptId = null)
        {
            var db = _uow.DbContext;

            //是否為素人
            var checkIsAmateur = (from o in db.Organizations
                                  join m in db.Members on o.Id equals m.OrgId
                                  where o.OrgCode == "amateur" && m.Id == memberId && m.RoleName == "2"
                                  select m).FirstOrDefault();


            var query = (checkIsAmateur != null)
                ?  //是素人老師
                    (from lc in db.LearningCircle
                     where lc.CreateUser == memberId && lc.Enable == true && lc.Visibility == true

                     select new Infrastructure.ViewModel.Backend.LearningCircleManageViewModel
                     {
                         Description = lc.Description,
                         EndDate = lc.EndDate.Value,
                         Enable = lc.Enable.ToString(),
                         LearningOuterKey = lc.LearningOuterKey,
                         Name = lc.Name,
                         OrgId = lc.OrgId.Value.ToString(),
                         Section = lc.Section.ToString(),
                         StartDate = lc.StartDate.Value
                     }).ToList()
                  : //是有學校組織 或 系統管理者
                    //有查詢學院條件
                        (deptId.HasValue)
                              ? ((from lc in db.LearningCircle
                                  join o in db.Organizations on lc.OrgId equals o.Id
                                  join c in db.Courses on lc.CourseId equals c.Id
                                  where //是否為某個學院的編號
                                              c.DeptId == deptId.Value && lc.Enable == true && lc.Visibility == true && lc.OrgId == orgId
                                  select new Infrastructure.ViewModel.Backend.LearningCircleManageViewModel
                                  {
                                      Description = lc.Description,
                                      EndDate = lc.EndDate.Value,
                                      Enable = lc.Enable.ToString(),
                                      LearningOuterKey = lc.LearningOuterKey,
                                      Name = lc.Name,
                                      OrgId = lc.OrgId.Value.ToString(),
                                      Section = lc.Section.ToString(),
                                      StartDate = lc.StartDate.Value
                                  }).ToList())
                              //沒有查詢學院條件
                              : ((from lc in db.LearningCircle
                                  //      join c in db.Courses on lc.CourseId equals c.Id
                                  //      join d in db.Depts on c.DeptId equals d.Id
                                  join o in db.Organizations on lc.OrgId equals o.Id
                                  join m in db.Members on o.Id equals m.OrgId
                                  where m.Id == memberId && lc.Enable == true && lc.Visibility == true && lc.OrgId == orgId
                                  select new Infrastructure.ViewModel.Backend.LearningCircleManageViewModel
                                  {
                                      Description = lc.Description,
                                      EndDate = lc.EndDate.Value,
                                      Enable = lc.Enable.ToString(),
                                      LearningOuterKey = lc.LearningOuterKey,
                                      Name = lc.Name,
                                      OrgId = lc.OrgId.Value.ToString(),
                                      Section = lc.Section.ToString(),
                                      StartDate = lc.StartDate.Value
                                  }).ToList());

            var weekTableService = new WeekTableService();
            //重新塞weeks資料
            foreach (var learningCircleData in query)
            {
                learningCircleData.StartDate = learningCircleData.StartDate.ToLocalTime();
                learningCircleData.EndDate = learningCircleData.EndDate.ToLocalTime();
                var weekDatas = weekTableService.GetByCirclekey(learningCircleData.LearningOuterKey);
                if (weekDatas != null && weekDatas.WeekTable.FirstOrDefault() != null)
                {
                    learningCircleData.Place = weekDatas.WeekTable.FirstOrDefault().Place;
                    learningCircleData.Weeks = weekDatas.WeekTable.Select(t => (int)t.StartTime.DayOfWeek).ToList();
                }
            }
            return (searchString != string.Empty && searchString != null) ? query.Where(t => t.Name.Contains(searchString) || t.LearningOuterKey.Contains(searchString)).OrderBy(t => t.StartDate).ToList() : query.OrderBy(t => t.StartDate).ToList();
        }
        /// <summary>
        ///  取得某使用者的所有身分學習圈
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.Entity.LearningCircle> GetLearningCircleListByToken(string token, int? orgId, string searchName)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            var result = GetLearningCircleListByMemberId(memberInfo.Id, orgId, searchName);
            return result;
        }

        /// <summary>
        ///  取得某使用者的所有身分學習圈
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public IEnumerable<LearningCircle> GetLearningCircleListByMemberId(int memberId, int? orgId, string searchName)
        {
            var db = _uow.DbContext;
            //後臺查詢
            if (orgId.HasValue && orgId.Value > 0)
            {

            }
            var response = (from lc in db.LearningCircle
                            join cm in db.CircleMemberRoleplay on lc.Id equals cm.CircleId
                            where cm.MemberId == memberId && lc.Enable == true
                            group lc by new { lc } into g
                            select g.Key.lc).ToList();
            if (response.Count() <= 0)
                return null;
            return response;
        }


        /// <summary>
        /// 取得APP的GetCourseDetail的資訊
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.CourseManage.GetCourseDetailResponse APPGetCourseDetail(string token, string circleKey)
        {
            var response = new Infrastructure.ViewModel.CourseManage.GetCourseDetailResponse();
            var memberService = new MemberService();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token);
            if (checkToken == null)
                return response;

            var members = memberService.GetLearningCircleMembers(circleKey);
            if (members == null)
                return response;

            response.MemberCount = members.Count();
            response.Edit = false;
            response.EditImpression = false;
            response.CollInfo = null;

            var learningCircleInfo = GetDetailByOuterKey(circleKey);

            if (learningCircleInfo == null)
                return response;

            //處理老師名單
            var teachers = memberService.GetTeacherList(circleKey);
            if (teachers.FirstOrDefault() != null)
            {
                var teachersName = teachers.Select(t => t.Name).ToList();
                foreach (var teacher in teachersName)
                {
                    response.ClassTeachers += teacher + "、";
                }
                response.ClassTeachers = response.ClassTeachers.Substring(0, response.ClassTeachers.Length - 1);
            }
            if (learningCircleInfo.StartDate.HasValue && learningCircleInfo.EndDate.HasValue)
            {
                response.ClassPeriod = string.Format("{0} ~ {1}", learningCircleInfo.StartDate.Value.ToLocalTime().ToString("yyyy/MM/dd"), learningCircleInfo.EndDate.Value.ToLocalTime().ToString("yyyy/MM/dd"));
                response.StartDate = learningCircleInfo.StartDate.Value.ToLocalTime();
                response.EndDate = learningCircleInfo.EndDate.Value.ToLocalTime();
            }

            var weekTableService = new WeekTableService();
            var ClassWeekTableDatas = weekTableService.GetByCirclekey(circleKey);
            response.WeekTable = new List<Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable>();
            if (ClassWeekTableDatas.WeekTable.FirstOrDefault() != null)
            {
                foreach (var weekTableData in ClassWeekTableDatas.WeekTable)
                {
                    var data = new Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable()
                    {
                        Week = weekTableData.Week,
                        Place = weekTableData.Place,
                        StartPeriod = weekTableData.StartPeriod.Value,
                        EndPeriod = weekTableData.EndPeriod.Value,
                        StartTime = string.Format("{0:HH}:{0:mm}", weekTableData.StartTime.ToLocalTime()),
                        EndTime = string.Format("{0:HH}:{0:mm}", weekTableData.EndTime.ToLocalTime())
                    };
                    response.WeekTable.Add(data);
                }
            }

            response.ClassSubjectName = learningCircleInfo.Name;
            response.ClassId = learningCircleInfo.LearningOuterKey;
            response.Introduction = learningCircleInfo.Description;
            response.ClassName = learningCircleInfo.Name;
            response.ClassDomainId = null;
            response.ClassDomainName = null;
            response.Note = learningCircleInfo.ReMark;
            response.ClassTarget = learningCircleInfo.Objective;
            return response;
        }

        /// <summary>
        /// 取得APP的GetAllMyCourse的資訊
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse> APPGetAllMyCourse(string token)
        {
            var memberService = new MemberService();
            var checkToken = memberService.TokenToMember(token).Result;
            if (checkToken == null)
                return null;

            var sectionService = new SectionService();
            var sectionData = sectionService.GetOrgNowSeme(checkToken.OrgId);


            var learningCircleList = GetLearningCircleListByToken(token, null, null);
            if (learningCircleList == null || learningCircleList.Count() <= 0)
                return new List<Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse>();
            var response = new List<Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse>();
            var sectionYear = learningCircleList.Where(t => t.StartDate.HasValue).GroupBy(t => t.StartDate.Value.Year);
            //上過多少學年度
            foreach (var year in sectionYear)
            {
                var data = new Infrastructure.ViewModel.CourseManage.GetAllMyCourseResponse();
                var courseData = new List<Infrastructure.ViewModel.CourseManage.DataCourseDataModel>();
                data.Year = year.Key;
                data.YearSeme = year.Key.ToString();
                if (year.Key == sectionData.Year)
                    data.IsNowSeme = true;
                //查詢課程
                foreach (var learningcircle in learningCircleList.Where(t => t.StartDate.HasValue && ((t.StartDate >= sectionData.StartDate && t.StartDate <= sectionData.EndDate) || (t.EndDate >= sectionData.StartDate && t.EndDate <= sectionData.EndDate))))
                {
                    var learningCircleData = new Infrastructure.ViewModel.CourseManage.DataCourseDataModel();

                    learningCircleData.ClassId = learningcircle.LearningOuterKey.ToLower();
                    learningCircleData.ClassName = learningcircle.Name;
                    learningCircleData.ClassSubjectName = learningcircle.Name;
                    //查詢課程老師們
                    var teachers = memberService.GetTeacherList(learningcircle.LearningOuterKey);
                    var teacherListData = new List<Infrastructure.ViewModel.MemberManage.TeacherPhotoInfo>();
                    var teacherNames = string.Empty;
                    //設定上課老師們資料
                    foreach (var teacher in teachers)
                    {
                        var teacherData = new Infrastructure.ViewModel.MemberManage.TeacherPhotoInfo();
                        teacherData.Email = teacher.Email;
                        teacherData.ManName = teacher.Name;
                        teacherData.Url = teacher.Photo;
                        teacherListData.Add(teacherData);
                        teacherNames += teacher.Name + ",";
                    }
                    teacherNames = teacherNames != string.Empty ? teacherNames.Substring(0, teacherNames.Length - 1) : teacherNames;
                    learningCircleData.TeacherPhoto = teacherListData.ToArray();
                    learningCircleData.ClassTeacher = teacherNames;
                    learningCircleData.StartDate = learningcircle.StartDate.HasValue ? learningcircle.StartDate.Value.ToLocalTime() : DateTime.MinValue;
                    learningCircleData.EndDate = learningcircle.EndDate.HasValue ? learningcircle.EndDate.Value.ToLocalTime() : DateTime.MinValue;
                    var memberCount = memberService.GetLearningCircleMembers(learningcircle.LearningOuterKey).Count();
                    learningCircleData.MemberCount = memberCount;
                    var weekTableService = new WeekTableService();
                    var weekDatas = weekTableService.GetAllMyCourseWeekTableData(learningcircle.Id);
                    if (weekDatas != null)
                        learningCircleData.WeekTable = weekDatas.ToList();
                    courseData.Add(learningCircleData);
                }
                data.Course = courseData.ToArray();
                response.Add(data);
            }
            return response;
        }
        /// <summary>
        /// 取得學習圈所有成員帳號
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public List<string> GetCircleMemberList(string circleKey, int? myId = null)
        {
            var cacheService = new CacheService();
            // 從快取取出學習圈成員列表
            var data = cacheService.GetCircleMember(circleKey);
            // 2016-9-20 add by sophiee:APP team告知，推播未註明接收對象的事件，代表發給該課程中的所有成員(除了自己)
            // 因此增加myId參數，如果有傳值，就特別排除掉自己
            if (myId.HasValue)
                data = data.Where(x => x.Id != myId).ToList();
            return data.Select(x => x.Account).ToList();
        }
        /// <summary>
        /// 取出學習圈成員ID列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public List<int> GetCircleMemberIdList(string circleKey, int? myId = null)
        {
            var cacheService = new CacheService();
            // 從快取取出學習圈成員列表
            var data = cacheService.GetCircleMember(circleKey);
            if (myId.HasValue)
            {
                var removeMy = data.FirstOrDefault(t => t.Id == myId.Value);
                data.Remove(removeMy);
            }

            return data.Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 取得某課程的學習圈
        /// </summary>
        /// <param name="course_no"></param>
        /// <returns></returns>
        public LearningCircle GetCourseLearningInfo(string course_no)
        {
            var db = _uow.DbContext;
            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.Equals(course_no));
            return (learningCircleInfo != null) ? (learningCircleInfo) : (new LearningCircle());
        }

        /// <summary>
        /// 建立學習圈 - [後臺管理API用]
        /// </summary>
        /// <returns></returns>
        public LearningCircle CreateLearningCircle(string name, string circleKey, string description, string token, DateTime startDate, DateTime endDate, int? orgId, string objective = null, int? section = null)
        {
            var db = _uow.DbContext;

            var checkDBEmpty = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == circleKey && t.Name == name);
            if (checkDBEmpty != null)
                return checkDBEmpty;
            var now = DateTime.UtcNow;
            var tokenService = new TokenService();
            var creator = tokenService.GetTokenInfo(token).Result;
            var entity = new LearningCircle()
            {
                Created = TimeData.Create(now),
                Deleted = TimeData.Create(null),
                Updated = TimeData.Create(null),
                CreateUser = creator.MemberId,
                Description = description,
                Name = name,
                Enable = true,
                Visibility = true,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                LCType = 10,
                Objective = objective
            };
            if (section.HasValue)
            {
                entity.Section = section.Value.ToString();
            }
            if (orgId.HasValue)
                entity.OrgId = orgId.Value;

            if (circleKey == null || circleKey == string.Empty)
                circleKey = string.Format("{0}{1}", now.Ticks.ToString(), "course");

            entity.LearningOuterKey = circleKey.ToLower();

            db.LearningCircle.Add(entity);
            db.SaveChanges();
            var memberInviteService = new MemberInviteService();
            var memerInviteInfo = memberInviteService.CreateFromCreateLearningCircle(circleKey.ToLower());
            return entity;
        }

        /// <summary>
        /// 編輯學習圈 - [後臺管理API用]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="circleKey"></param>
        /// <param name="description"></param>
        /// <param name="token"></param>
        /// <param name="enable"></param>
        /// <param name="objective"></param>
        /// <returns></returns>
        public LearningCircle UpdateLearningCircle(string name, string circleKey, string description, string token, bool enable, string objective, string remark)
        {
            var db = _uow.DbContext;
            var checkDBEmpty = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == circleKey);
            if (checkDBEmpty == null)
                return null;

            var tokenService = new TokenService();
            var updater = tokenService.GetTokenInfo(token).Result;
            checkDBEmpty.Updated = TimeData.Create(DateTime.UtcNow);
            checkDBEmpty.UpdateUser = updater.MemberId;
            checkDBEmpty.Description = description;
            checkDBEmpty.Name = name;
            checkDBEmpty.Enable = enable;
            checkDBEmpty.Objective = objective;
            checkDBEmpty.ReMark = remark;
            // var timeTableService = new Backend.TimeTableService();
            //如果有修改開始日期與修改結束日期
            /*  if (startDate.HasValue && endDate.HasValue)
              {
                  //需要修改 weekTable資料
                  if (weeks !=null && weeks.Count > 0)
                  {
                      var weekTableService = new WeekTableService();
                      var updateWeekDatas = weekTableService.UpdateWeekDatas(token, circleKey, place, startDate.Value, endDate.Value,0,weeks);
                  }
                  checkDBEmpty.StartDate = startDate.Value.ToUniversalTime();
                  checkDBEmpty.EndDate = endDate.Value.ToUniversalTime();
                  var updateResponse = timeTableService.UpdateByCircleKey(startDate.Value.ToUniversalTime(), endDate.Value.ToUniversalTime(), circleKey);
              }
              //如果只有修改結束日期
              else if (endDate.HasValue)
              {
                  //需要修改 weekTable資料
                  if (weeks != null && weeks.Count > 0)
                  {
                      var weekTableService = new WeekTableService();
                      var updateWeekDatas = weekTableService.UpdateWeekDatas(token, circleKey, place, checkDBEmpty.StartDate.Value, endDate.Value, 0, weeks);
                  }
                  checkDBEmpty.EndDate = endDate.Value.ToUniversalTime();

                  var updateResponse = timeTableService.UpdateByCircleKey(checkDBEmpty.StartDate.Value, endDate.Value.ToUniversalTime(), circleKey);
              }
              //如果只有修改開始日期
              else if (startDate.HasValue)
              {
                  //需要修改 weekTable資料
                  if (weeks != null && weeks.Count > 0)
                  {
                      var weekTableService = new WeekTableService();
                      var updateWeekDatas = weekTableService.UpdateWeekDatas(token, circleKey, place, checkDBEmpty.StartDate.Value, checkDBEmpty.EndDate.Value, 0, weeks);
                  }
                  checkDBEmpty.StartDate = startDate.Value.ToUniversalTime();
                  var updateResponse = timeTableService.UpdateByCircleKey(startDate.Value.ToUniversalTime(), checkDBEmpty.EndDate.Value, circleKey);
              }*/
            db.SaveChanges();

            return checkDBEmpty;
        }

        /// <summary>
        /// 建立學習圈 - [後臺管理-Area]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public LearningCircle InsertLearningCircle(Infrastructure.ViewModel.Backend.LearningCircleViewModel data, int creator)
        {
            try
            {
                var learningRoleService = new LearningRoleService();
                var authService = new AuthService();
                var db = _uow.DbContext;
                var entity = data.LearningInfo;
                entity.Updated = TimeData.Create(null);
                entity.Deleted = TimeData.Create(null);
                entity.Created = TimeData.Create(DateTime.UtcNow);
                entity.CreateUser = creator;
                entity.LCType = 10;
                entity.Visibility = true;
                db.LearningCircle.Add(entity);
                db.SaveChanges();

                //建立老師角色身分
                var teacherRole = learningRoleService.AddLearningEditRole(creator, entity.Id, "老師", true, false, 1);
                //建立助教角色身分
                var surpportTeacherRole = learningRoleService.AddLearningEditRole(creator, entity.Id, "助教", true, false, 2);
                //建立學生角色身分
                var studentRole = learningRoleService.AddLearningEditRole(creator, entity.Id, "學生", false);
                //建立學習圈所有角色的權限
                var insertAuthSuccess = authService.InsertLearningCircleAllRoleAuth(entity.Id, creator);

                # region 新增擴充欄位資訊[目前只有文大有] - 如果是新的學校需要設定新增擴充欄位 [ExtensionColumns資料表]才會有
                if (data.ExtensionInfo != null)
                {
                    foreach (var row in data.ExtensionInfo)
                    {
                        var extValueEntity = new LCExtensionValue();
                        extValueEntity.DataId = entity.Id;
                        extValueEntity.TextValue = row.Value.TextValue;
                        extValueEntity.ColumnId = row.Column.Id;
                        db.LCExtensionValue.Add(extValueEntity);
                    }
                    db.SaveChanges();
                }

                #endregion

                return entity;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        /// <summary>
        /// 編輯學習圈 - 單純建立學習圈本身
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public LearningCircle UpdateLearningCircle(Infrastructure.ViewModel.Backend.LearningCircleViewModel data, int updater)
        {
            try
            {
                var learningRoleService = new LearningRoleService();
                var authService = new AuthService();
                var db = _uow.DbContext;

                var entity = db.LearningCircle.FirstOrDefault(t => t.Id == data.LearningInfo.Id);
                if (entity == null)
                    return null;

                entity.Name = data.LearningInfo.Name;
                entity.Description = data.LearningInfo.Description;
                entity.Updated = TimeData.Create(DateTime.UtcNow);
                entity.Deleted = TimeData.Create(null);
                entity.UpdateUser = updater;
                db.SaveChanges();

                if (data.ExtensionInfo != null)
                {
                    foreach (var extInfo in data.ExtensionInfo)
                    {
                        var dbExtInfo = db.LCExtensionValue.FirstOrDefault(t => t.DataId == entity.Id && t.ColumnId == extInfo.Column.Id);
                        dbExtInfo.TextValue = extInfo.Value.TextValue;
                    }
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

        /// <summary>
        /// 刪除學習圈 - 刪除標記'
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteLearningCircleByCircleKey(string circleKey, string token)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return false;

            var db = _uow.DbContext;
            var entity = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == circleKey && t.Enable == true && t.Visibility == true);
            if (entity == null)
                return false;
            else
            {
                entity.Enable = false;
                entity.Visibility = false;
                entity.Deleted = TimeData.Create(DateTime.UtcNow);
                entity.DeleteUser = tokenInfo.MemberId;
                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 刪除學習圈 - 補上刪除標記
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public bool DeleteLearningCircle(int learningCircleId, int deleter)
        {
            var db = _uow.DbContext;
            var entity = db.LearningCircle.FirstOrDefault(t => t.Id == learningCircleId && t.Enable == true && t.Visibility == true);
            if (entity == null)
                return false;
            else
            {
                entity.Enable = false;
                entity.Visibility = false;
                entity.Deleted = TimeData.Create(DateTime.UtcNow);
                entity.DeleteUser = deleter;
                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 直接抓db的老師資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public List<int> GetCircleTeacherIdListBySql(string circleKey, int? myId)
        {  // 從快取取出學習圈成員列表
            var db = _uow.DbContext;
            var data = (from c in db.LearningCircle
                        join cmr in db.CircleMemberRoleplay on c.Id equals cmr.CircleId
                        join m in db.Members on cmr.MemberId equals m.Id
                        join lr in db.LearningRole on cmr.RoleId equals lr.Id
                        where c.LearningOuterKey == circleKey && c.Enable == true && lr.IsAdminRole == true
                        select new MemberCacheData { Id = m.Id, Account = m.Account }).ToList();
            // var data = GetCircleTeacherMember(circleKey);

            // 2016-9-20 add by sophiee:APP team告知，推播未註明接收對象的事件，代表發給該課程中的所有成員(除了自己)
            // 因此增加myId參數，如果有傳值，就特別排除掉自己
            if (myId.HasValue)
            {
                data = data.Where(x => x.Id != myId).ToList();
            }

            return data.Select(x => x.Id).ToList();
        }

        /// <summary>
        /// 直接抓db的老師資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public List<string> GetCircleTeacherListBySql(string circleKey, int? myId)
        {  // 從快取取出學習圈成員列表
            var db = _uow.DbContext;
            var data = (from c in db.LearningCircle
                        join cmr in db.CircleMemberRoleplay on c.Id equals cmr.CircleId
                        join m in db.Members on cmr.MemberId equals m.Id
                        join lr in db.LearningRole on cmr.RoleId equals lr.Id
                        where c.LearningOuterKey == circleKey && c.Enable == true && lr.IsAdminRole == true
                        select new MemberCacheData { Id = m.Id, Account = m.Account }).ToList();
            // var data = GetCircleTeacherMember(circleKey);

            // 2016-9-20 add by sophiee:APP team告知，推播未註明接收對象的事件，代表發給該課程中的所有成員(除了自己)
            // 因此增加myId參數，如果有傳值，就特別排除掉自己
            if (myId.HasValue)
            {
                data = data.Where(x => x.Id != myId).ToList();
            }

            return data.Select(x => x.Account).ToList();
        }
    }
}

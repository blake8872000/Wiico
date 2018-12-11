using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class CourseService
    {
        private readonly GenericUnitOfWork _uow;

        public CourseService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得某位老師所開的課程 - ByAccount
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<Course> GetTeachersCourseByToken(string token)
        {
            var db = _uow.DbContext;
            var memberService = new MemberService();
            var teacherInfo = memberService.TokenToMember(token);
            return teacherInfo != null ? GetTeachersCourseByMemberId(teacherInfo.Id) : null;
        }

        /// <summary>
        /// 取得某位老師所開的課程 - ByAccount
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<Course> GetTeachersCourseByAccount(string account)
        {
            var db = _uow.DbContext;
            var teacherInfo = db.Members.FirstOrDefault(t => t.Account == account);
            return teacherInfo != null ? GetTeachersCourseByMemberId(teacherInfo.Id) : null;
        }

        /// <summary>
        /// 取得某位老師所開的課程 - ByMemberId
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public IEnumerable<Course> GetTeachersCourseByMemberId(int memberId)
        {
            var db = _uow.DbContext;
            var courseList = from c in db.Courses
                             where c.CreateUser == memberId
                             select c;
            return courseList.ToList();
        }

        /// <summary>
        /// 取得學院列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<Dept> DeptAdminList(int memberId)
        {
            var db = _uow.DbContext;
            var result = new List<Dept>();
            var list = db.DepartmentAdmin.Where(t => t.MemberId.Equals(memberId) && t.Enable.Equals(true) && t.Visibility.Equals(true));
            if (!list.Any())
                return null;
            else
            {
                foreach (var _item in list)
                {
                    var deptInfo = db.Depts.Find(_item.DeptId);
                    result.Add(deptInfo);
                }
                return result;
            }
        }

        /// <summary>
        /// 查詢詳細的課程資訊
        /// </summary>
        /// <param name="course_no"></param>
        /// <returns></returns>
        //public Infrastructure.ViewModel.CourseListViewModel GetiCan5CourseInfo(int learningId)
        //{
        //    var result = new Infrastructure.ViewModel.CourseListViewModel();
        //    var db = _uow.DbContext;

        //    var courseList = _uow.CoursesRepo.GetCourseInfoList(learningId);

        //    foreach (var _item in courseList)
        //    {
        //        _item.ExtensionColumnInfo = new List<Infrastructure.ValueObject.ExtensionColumnValue>();

        //        var extensionDisplayName = from d in db.Depts
        //                                   join c in db.Courses on d.Id equals c.DeptId
        //                                   join ev in db.ExtensionValue on d.DeptCode equals ev.TextValue
        //                                   where c.CourseCode.Equals(_item.CourseCode)
        //                                   select d.Name;
        //        var showDisplayName = "";
        //        if (extensionDisplayName.Any())
        //        {
        //            showDisplayName = extensionDisplayName.ToList()[0];
        //        }
        //        //塞擴充欄位
        //        var extensionList = from c in db.Courses
        //                            join ev in db.ExtensionValue on c.Id equals ev.DataId
        //                            join ec in db.ExtensionColumn on ev.ColumnId equals ec.Id
        //                            where c.CourseCode.Equals(_item.CourseCode)
        //                            select new Infrastructure.ValueObject.ExtensionColumnValue
        //                            {
        //                                ExtensionColumnId = ec.Id,
        //                                ExtensionColumnName = ec.Name,
        //                                Value = ev.TextValue,
        //                                DispalyName = showDisplayName
        //                            };
        //        _item.ExtensionColumnInfo = extensionList.ToList();

        //        //塞課程進度
        //        _item.Syllabus = new List<Syllabus>();
        //        var sbResult = from sb in db.Syllabus
        //                       where sb.Course_No.Equals(_item.CourseCode) && sb.Enable.Equals(true)
        //                       orderby sb.Syll_Date, sb.Sort
        //                       select sb;
        //        _item.Syllabus = sbResult.ToList();

        //        //塞上課教室跟時間資訊
        //        _item.TimeTable = new List<TimeTable>();

        //        var ttResult = from tt in db.TimeTable
        //                       join c in db.Courses on tt.Course_No equals c.CourseCode
        //                       join ev in db.ExtensionValue on c.Id equals ev.DataId
        //                       join ec in db.ExtensionColumn on ev.ColumnId equals ec.Id
        //                       where c.CourseCode.Equals(_item.CourseCode) && ev.ColumnId.Equals(26) //26代表cour_timeTable
        //                       select tt;
        //        _item.TimeTable = ttResult.ToList();

        //        //查詢老師有誰
        //        var teacherslist = _uow.IThinkVmRepo.GetLearningCircleMemberInfo(_item.CircleId);
        //        _item.Teachers = teacherslist.ToList();

        //        //查詢點名數量
        //        var sqlSignCount = from table in db.ActRollCall where table.LearningId.Equals(_item.CircleId) select learningId;
        //        _item.SignCount = sqlSignCount.Count();

        //        //查詢作業數量
        //        var sqlHomeCount = from table in db.ActHomeWork where table.LearningId.Equals(_item.CircleId) select learningId;
        //        _item.HomeWorkCount = sqlHomeCount.Count();
        //    }
        //    result.list = courseList;
        //    return result;
        //}


        /// <summary>
        /// 同步iCan5課程
        /// </summary>
        /// <param name="course_no"></param>
        public void CheckCourseInfo(string course_no)
        {
            _uow.CoursesRepo.CheckCourseInfo(course_no);
        }

        /// <summary>
        /// 取得根據課程編號課程資料
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Course GetCourseById(int courseId)
        {
            var db = _uow.DbContext;
            return db.Courses.Find(courseId);
        }

        /// <summary>
        /// 取得課程擴充欄位資訊 - For後臺管理專用
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Backend.CourseViewModel GetCourseExtensionInfo(int courseId)
        {
            var db = _uow.DbContext;
            //Course ViewModel
            var result = new Infrastructure.ViewModel.Backend.CourseViewModel();

            //viewModel.CourseInfo 課程基本欄位
            var courseInfo = db.Courses.Find(courseId);
            if (courseInfo == null)
                return null;

            //ExtensionInfo 課程擴充欄位
            var cExt = from ev in db.ExtensionValue
                       join ec in db.ExtensionColumn on ev.ColumnId equals ec.Id
                       orderby ec.Sort
                       where ev.DataId == courseInfo.Id
                       select new Infrastructure.ViewModel.Backend.CourseExt
                       {
                           ColumnsId = ev.Id,
                           ColumnsName = ec.DisplayName,
                           Value = ev.TextValue,
                           Sort = ec.Sort,
                           EditorMultiLine = ec.EditorMultiLine,
                           DisplayMultiLine = ec.DisplayMultiLine,
                           EditorMaxLength = ec.EditorMaxLength,
                           Enable = ec.Enable,
                           Editable = ec.Editable
                       };

            result.CourseInfo = courseInfo;
            result.ExtensionInfo = cExt.ToList();
            return result;
        }

        /// <summary>
        /// 建立一個課程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="courseCode"></param>
        /// <param name="outline"></param>
        /// <param name="memberId"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public Infrastructure.Entity.Course InsertCourse(string name, string courseCode, string outline, int memberId, int? deptId)
        {

            try
            {
                var db = _uow.DbContext;
                var entity = new Infrastructure.Entity.Course()
                {
                    Name = name,
                    CourseCode = courseCode,
                    CourseOutline = outline,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    CreateUser = memberId
                };
                int? nil = null;
                entity.DeptId = deptId.HasValue ? deptId.Value : nil;

                db.Courses.Add(entity);
                db.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 取得課程擴充欄位資訊列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.Backend.CourseExt> GetCourseExtensionColumns(int orgId)
        {
            var result = new List<Infrastructure.ViewModel.Backend.CourseExt>();
            var db = _uow.DbContext;
            var dbDatas = db.ExtensionColumn.Where(t => t.OrgId == orgId && t.Enable == true).OrderBy(t => t.Sort).ToList();

            //取出Enable=True並排序
            foreach (var _item in dbDatas)
            {
                var _info = new Infrastructure.ViewModel.Backend.CourseExt();
                _info.ColumnsId = _item.Id;
                _info.ColumnsName = _item.Name;
                _info.DisplayName = _item.DisplayName;
                _info.Sort = _item.Sort;
                _info.EditorMultiLine = _item.EditorMultiLine;
                _info.DisplayMultiLine = _item.DisplayMultiLine;
                _info.EditorMaxLength = _item.EditorMaxLength;
                _info.Editable = _item.Editable;

                result.Add(_info);
            }
            return result;
        }

        /// <summary>
        /// 建立課程的擴充欄位資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public bool InsertCourseExtensionInfo(List<Infrastructure.ViewModel.Backend.CourseExt> datas, int courseId)
        {
            //不需新增擴充欄位 直接跳離開
            if (datas == null)
                return true;

            try
            {
                var db = _uow.DbContext;


                foreach (var row in datas)
                {
                    var _courseExtValue = new ExtensionValue();

                    _courseExtValue.DataId = courseId;
                    _courseExtValue.TextValue = row.Value;
                    _courseExtValue.ColumnId = row.ColumnsId;

                    db.ExtensionValue.Add(_courseExtValue);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新課程資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public bool UpdateCourseInfo(Infrastructure.ViewModel.Backend.CourseViewModel datas)
        {
            try
            {
                var db = _uow.DbContext;
                var dbCourse = db.Courses.Find(datas.CourseInfo.Id);
                if (dbCourse == null)
                    return false;

                dbCourse.Updated = TimeData.Create(DateTime.UtcNow);
                dbCourse.UpdateUser = dbCourse.CreateUser;
                dbCourse.Name = datas.CourseInfo.Name;
                dbCourse.CourseOutline = datas.CourseInfo.CourseOutline;
                dbCourse.CourseCode = datas.CourseInfo.CourseCode;

                //db.Entry(datas.CourseInfo).State = EntityState.Modified;

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 刪除課程
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public bool DeleteCourseInfoById(int courseId)
        {
            var db = _uow.DbContext;
            var course = db.Courses.Find(courseId);
            if (course == null)
                return false;

            db.Courses.Remove(course);
            db.SaveChanges();
            return true;
        }
    }
}


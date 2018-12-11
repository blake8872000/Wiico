using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class TimeTableService
    {
        private readonly GenericUnitOfWork _uow = new GenericUnitOfWork();

        public TimeTableService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得上課列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.Entity.TimeTable> GetList(string circleKey)
        {
            var db = _uow.DbContext;
            var response = (from tt in db.TimeTable
                            join lc in db.LearningCircle on tt.Course_No equals lc.LearningOuterKey
                            where tt.Course_No == circleKey
                            orderby tt.StartDate
                            select tt).ToList();
            if (response.Count() <= 0)
                return null;

            foreach (var item in response)
            {
                item.StartDate = item.StartDate.Value.ToLocalTime();
                item.EndDate = item.EndDate.Value.ToLocalTime();
            }

            return response;
        }

        /// <summary>
        /// 取得上課時間列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.CourseManage.GetWeekBySemeResponse> APPGetWeekBySeme(string token)
        {
            var tokenService = new TokenService();
            var memberService = new MemberService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            if (checkToken == null)
                return null;
            var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);
            if (memberInfo == null)
                return null;

            var db = _uow.DbContext;
            var sectionData = db.Sections.FirstOrDefault(t => t.IsNowSeme == true && t.OrgId == memberInfo.OrgId);
            if (sectionData == null)
                return null;

            var dateTimeTools = new Utility.DateTimeTools();
            var responseData = dateTimeTools.GetIntervalDateList(sectionData.StartDate.ToLocalTime(), sectionData.EndDate.ToLocalTime(), 7);
            return responseData.OrderBy(t => t.Start_date);
        }

        /// <summary>
        /// 取得使用者所有上課時間地點
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse> AppGetMyCourseRoomSchedule(string token)
        {
            var tokenService = new TokenService();
            var memberService = new MemberService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            if (checkToken == null)
                return null;
            var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);
            if (memberInfo == null)
                return null;
            var db = _uow.DbContext;
            /*  var sqlData = (from ttb in db.TimeTable
                             join lc in db.LearningCircle on ttb.Course_No equals lc.LearningOuterKey
                             join wt in db.WeekTable on lc.Id equals wt.LearningCircleId
                             join cm in db.CircleMemberRoleplay on lc.Id equals cm.CircleId
                             orderby ttb.StartDate
                             group ttb by new { ttb, lc.LearningOuterKey, lc.Name, wt.StartPeriod, wt.EndPeriod, wt.Place, cm.MemberId, wt.ClassWeekType, lc.Enable ,wt.Week} into g
                             where g.Key.MemberId == memberInfo.Id && g.Key.Enable == true
                             select new Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse
                             {
                                 Id=g.Key.ttb.Id,
                                 RoomID = null,
                                 RoomName = g.Key.Place,
                                 ClassID = g.Key.LearningOuterKey,
                                 ClassName = g.Key.Name,
                                 StartDate = g.Key.ttb.StartDate.Value,
                                 EndDate = g.Key.ttb.EndDate.Value,
                                 StartPeriod = g.Key.StartPeriod.Value,
                                 EndPeriod = g.Key.EndPeriod.Value,
                                 NameOfWeekDay = g.Key.Week,
                                 ClassWeekType = (Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse.enumClassWeekType)g.Key.ClassWeekType
                             }).ToList();*/

            var sqlData = (from wt in db.WeekTable
                           join lc in db.LearningCircle on wt.LearningCircleId equals lc.Id
                           join cmr in db.CircleMemberRoleplay on lc.Id equals cmr.CircleId
                           group wt by new { wt, lc.LearningOuterKey, lc.Name, cmr.MemberId, lc.Enable, lc.Section, lc.StartDate, lc.EndDate } into g
                           where g.Key.MemberId == memberInfo.Id &&
                                        g.Key.Enable == true
                           select new Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse
                           {
                               RoomID = null,
                               RoomName = g.Key.wt.Place,
                               ClassID = g.Key.LearningOuterKey.ToLower(),
                               ClassName = g.Key.Name,
                               StartDate = g.Key.wt.StartTime,
                               EndDate = g.Key.wt.EndTime,
                               StartPeriod = g.Key.wt.StartPeriod.Value,
                               EndPeriod = g.Key.wt.EndPeriod.Value,
                               NameOfWeekDay = g.Key.wt.Week,
                               ClassWeekType = (Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse.enumClassWeekType)g.Key.wt.ClassWeekType
                           }).ToList();
            if (sqlData.Count() <= 0)
                return null;
            var response = new List<Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse>();
            var dayOfWeek = new Utility.DayOfWeekTools();
            var dateTimeTools = new Utility.DateTimeTools();

            //weekTable資料
            foreach (var data in sqlData)
            {
                var timeDatas = dateTimeTools.GetIntervalDateList(data.StartDate, data.EndDate, 7);
                //塞TimeTable資料
                foreach (var timeData in timeDatas)
                {
                    var endDate = timeData.Start_date.Date;
                    endDate = endDate.AddHours(timeData.End_date.ToLocalTime().Hour);
                    endDate = endDate.AddMinutes(timeData.End_date.ToLocalTime().Minute);
                    var responseData = new Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse()
                    {
                        ClassID = data.ClassID,
                        ClassName = data.ClassName,
                        ClassWeekType = data.ClassWeekType,
                        EndDate = endDate,
                        StartDate = timeData.Start_date.ToLocalTime(),
                        StartPeriod = data.StartPeriod,
                        EndPeriod = data.EndPeriod,
                        RoomName = data.RoomName,
                        WeekDay = dayOfWeek.ChangeToDayOfWeek(data.NameOfWeekDay).Value,
                        NameOfWeekDay = data.NameOfWeekDay,
                        StartPeriodTime = timeData.Start_date.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                        EndPeriodTime = endDate.ToString("yyyy-MM-ddTHH:mm:ss")
                    };
                    response.Add(responseData);
                }
                /*    data.EndDate = data.EndDate.ToLocalTime();
                    data.StartDate = data.StartDate.ToLocalTime();
                    if (data.StartDate.DayOfWeek == dayOfWeek.ChangeToDayOfWeek(data.NameOfWeekDay)) {
                        //data.ClassID = data.ClassID.ToLower();
                        data.StartPeriodTime = data.StartDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        data.EndPeriodTime = data.EndDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        data.WeekDay = data.StartDate.DayOfWeek;
                        data.NameOfWeekDay = dayOfWeek.ChangeToCht(data.StartDate.DayOfWeek);
                        response.Add(data);
                    }*/
            }
            return response;
        }

        /// <summary>
        /// 取得上課列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse> APPGetMyCourseSchedule(string token)
        {
            var tokenService = new TokenService();
            var memberService = new MemberService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            if (checkToken == null)
                return null;
            var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);
            if (memberInfo == null)
                return null;

            var sectionService = new SectionService();
            var sectionInfo = sectionService.GetOrgNowSeme(memberInfo.OrgId);
            if (sectionInfo == null)
                return null;
            var nowSection = string.Format("{0}{1}", sectionInfo.FullName, sectionInfo.Serial);
            var db = _uow.DbContext;

            var sqlData = (from wt in db.WeekTable
                           join lc in db.LearningCircle on wt.LearningCircleId equals lc.Id
                           join cm in db.CircleMemberRoleplay on lc.Id equals cm.CircleId
                           where cm.MemberId == memberInfo.Id && lc.Enable == true && lc.Section == nowSection
                           orderby wt.StartTime
                           select new Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse
                           {
                               RoomID = null,
                               RoomName = wt.Place,
                               ClassID = lc.LearningOuterKey,
                               ClassName = lc.Name,
                               StartDate = wt.StartTime,
                               EndDate = wt.EndTime,
                               StartPeriod = wt.StartPeriod.Value,
                               EndPeriod = wt.EndPeriod.Value,
                               ClassWeekType = (Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse.enumClassWeekType)wt.ClassWeekType
                           }).ToList();
            if (sqlData.Count() <= 0)
                return null;
            var response = new List<Infrastructure.ViewModel.CourseManage.GetMyCourseScheduleResponse>();
            var dayOfWeek = new Utility.DayOfWeekTools();
            foreach (var data in sqlData)
            {
                data.EndDate = data.EndDate.ToLocalTime();
                data.StartDate = data.StartDate.ToLocalTime();

                data.StartPeriodTime = string.Format("{0:HH}:{0:mm}", data.StartDate);
                data.EndPeriodTime = string.Format("{0:HH}:{0:mm}", data.EndDate);
                data.WeekDay = data.StartDate.DayOfWeek;
                data.NameOfWeekDay = dayOfWeek.ChangeToCht(data.StartDate.DayOfWeek);
                response.Add(data);
            }
            return response;
        }

        /// <summary>
        /// 取得下一次上課時間地點
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public TimeTable Get(string circleKey)
        {
            var db = _uow.DbContext;
            var datas = db.TimeTable.Where(t => t.Course_No.ToLower() == circleKey).OrderByDescending(t => t.EndDate);
            var response = datas.FirstOrDefault(t => t.EndDate >= DateTime.UtcNow);
            if (response == null)
                return datas.FirstOrDefault();

            return response;
        }

        /// <summary>
        /// 根據課程資訊建立每日課表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="classWeekType"></param>
        /// <returns></returns>
        public bool CreateByCircleKey(string circleKey, int? classWeekType = 0)
        {

            var learningcircleService = new LearningCircleService();
            var learningcircleInfo = learningcircleService.GetDetailByOuterKey(circleKey);
            if (learningcircleInfo == null)
                return false;

            var dateTimeTools = new Utility.DateTimeTools();
            var datas = dateTimeTools.Getdata(learningcircleInfo.StartDate.Value.ToLocalTime(), learningcircleInfo.EndDate.Value.ToLocalTime(), circleKey, classWeekType);
            if (datas != null && datas.FirstOrDefault() != null)
            {
                var db = _uow.DbContext;
                db.TimeTable.AddRange(datas);
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 修改開始結束日期資訊
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public bool UpdateByCircleKey(DateTime startDate, DateTime endDate, string circleKey)
        {
            var learningcircleService = new LearningCircleService();
            var learningcircleInfo = learningcircleService.GetDetailByOuterKey(circleKey);
            if (learningcircleInfo == null)
                return false;
            var dateTimeTools = new Utility.DateTimeTools();
            var datas = dateTimeTools.Getdata(learningcircleInfo.StartDate.Value.ToLocalTime(), learningcircleInfo.EndDate.Value.ToLocalTime(), circleKey);
            //  var removeDatas   = datas.Where(t => t.StartDate.Value < startDate || t.EndDate.Value > endDate).ToList();
            var db = _uow.DbContext;
            var dbData = db.TimeTable.Where(t => t.Course_No == circleKey).ToList(); //取得DB目前所有上課時間地點資訊
            var allData = dbData.Union(datas); //聯集需求的日期
            var deleteDatas = allData.Except(datas); //找出不是需求的日期 -與DB資料的差集
            //找出不是需求的日期 
            var removeDatas = db.TimeTable.Where(t => t.Course_No == circleKey && (t.StartDate.Value < startDate || t.EndDate.Value > endDate)).ToList();
            removeDatas = removeDatas.Union(deleteDatas).ToList(); //聯集需要刪除的資料
            //刪除更新設定區間外的日期
            db.TimeTable.RemoveRange(removeDatas);
            //取得最初的開始日期
            var firstTimeTable = datas.OrderBy(t => t.StartDate).FirstOrDefault();
            //取得最後的結束日期
            var lastTimeTable = datas.OrderByDescending(t => t.EndDate).FirstOrDefault();
            var weekTableService = new WeekTableService();
            var weekTableInfo = weekTableService.GetByCirclekey(circleKey);
            var weekOfDays = weekTableInfo.WeekTable.Select(t => t.StartTime.ToLocalTime().DayOfWeek.ToString()).OrderBy(t => t);
            //新增DB所沒有的startDate日期區塊
            if (firstTimeTable.StartDate.HasValue && (firstTimeTable.StartDate.Value >= startDate))
            {
                var insertStartDates = new List<TimeTable>();

                var diffStartDay = (firstTimeTable.StartDate.Value - startDate).Days + 1;
                for (var firstDay = 1; firstDay < diffStartDay; firstDay++)
                {
                    var lastEntity = new TimeTable()
                    {
                        ClassRoom = firstTimeTable.ClassRoom,
                        ClassTime = firstTimeTable.ClassTime,
                        Course_No = firstTimeTable.Course_No,
                        Course_Id = firstTimeTable.Course_Id,
                        EndDate = firstTimeTable.EndDate,
                        StartDate = firstTimeTable.StartDate
                    };
                    var lastStartDate = lastEntity.StartDate.Value.AddDays(-(firstDay));
                    var lastEndDate = lastEntity.EndDate.Value.AddDays(-(firstDay));
                    if (weekOfDays.FirstOrDefault(t => t == lastStartDate.DayOfWeek.ToString()) != null)
                    {
                        lastEntity.StartDate = lastStartDate;
                        lastEntity.EndDate = lastEndDate;
                        db.TimeTable.Add(lastEntity);
                    }
                }
            }

            //新增DB所沒有的endDate日期 區塊
            if (lastTimeTable.EndDate.HasValue && (lastTimeTable.EndDate.Value >= endDate))
            {
                var insertStartDates = new List<TimeTable>();
                var diffEndDay = (endDate - lastTimeTable.EndDate.Value).Days + 1;
                for (var endDay = 1; endDay <= diffEndDay; endDay++)
                {
                    var nextEntity = new TimeTable()
                    {
                        ClassRoom = lastTimeTable.ClassRoom,
                        ClassTime = lastTimeTable.ClassTime,
                        Course_No = lastTimeTable.Course_No,
                        Course_Id = lastTimeTable.Course_Id,
                        EndDate = lastTimeTable.EndDate,
                        StartDate = lastTimeTable.StartDate
                    }; ;
                    var nextStartDate = nextEntity.StartDate.Value.AddDays(endDay);
                    var nextEndDate = nextEntity.EndDate.Value.AddDays(endDay);
                    if (weekOfDays.FirstOrDefault(t => t == nextStartDate.DayOfWeek.ToString()) != null)
                    {
                        nextEntity.StartDate = nextStartDate;
                        nextEntity.EndDate = nextEndDate;
                        db.TimeTable.Add(nextEntity);
                    }
                }
            }

            //補上新加入的weeks資料
            foreach (var data in datas)
            {
                var checkData = dbData.FirstOrDefault(t => t.StartDate.Value.Date == data.StartDate.Value.Date);
                if (checkData != null)
                    continue;

                db.TimeTable.Add(data);
            }


            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 處理資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool TimeTableDataProxy(TimeTablePostRequest requestData)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null)
                return false;
            var insertDatas = requestData.TimeTable.Where(t => t.Id == 0);
            //處理新增資料
            foreach (var insertData in insertDatas)
            {
                insertData.Course_No = requestData.CircleKey;
                insertData.StartDate = insertData.StartDate.Value.ToUniversalTime();
                insertData.EndDate = insertData.EndDate.Value.ToUniversalTime();
            }
            try
            {
                var db = _uow.DbContext;
                db.TimeTable.AddRange(insertDatas);
                var dbData = GetList(requestData.CircleKey.ToLower());
                //聯集資料
                var unionDataIds = requestData.TimeTable.Select(t => t.Id).Union(dbData.Select(t => t.Id));
                //找出欲刪除的資料
                var deleteDataIds = unionDataIds.Except(requestData.TimeTable.Select(t => t.Id));
                foreach (var deleteDataId in deleteDataIds)
                {
                    var deleteTimeTable = db.TimeTable.Find(deleteDataId);
                    if (deleteTimeTable == null)
                        continue;
                    db.TimeTable.Remove(deleteTimeTable);
                }
                //處理編輯資料
                var updateDatas = requestData.TimeTable.Where(t => t.Id > 0).ToList();
                foreach (var updateData in updateDatas)
                {
                    var updateDataInfo = db.TimeTable.Find(updateData.Id);
                    updateDataInfo.ClassRoom = updateData.ClassRoom;
                    updateDataInfo.StartDate = updateData.StartDate.Value.ToUniversalTime();
                    updateDataInfo.EndDate = updateData.EndDate.Value.ToUniversalTime();
                    updateDataInfo.Remark = updateData.Remark;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

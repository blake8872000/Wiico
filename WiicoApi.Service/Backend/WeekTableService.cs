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
    public class WeekTableService
    {
        private readonly GenericUnitOfWork _uow;
        public WeekTableService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得上課時間地點
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public WeekTableViewModel GetByCirclekey(string circleKey)
        {
            var db = _uow.DbContext;
            var learningcircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == circleKey);
            if (learningcircleInfo == null)
                return null;
            var response = new WeekTableViewModel();
            response.WeekTable = new List<WeekTable>();
            response.StartDate = learningcircleInfo.StartDate.Value.ToLocalTime();
            response.EndDate = learningcircleInfo.EndDate.Value.ToLocalTime();
            var weekTables = GetByLearningCircleId(learningcircleInfo.Id);
            if (weekTables != null)
            {
                if (weekTables.Count() > 0)
                    foreach (var weektable in weekTables)
                    {
                        weektable.StartTime = weektable.StartTime.ToLocalTime();
                        weektable.EndTime = weektable.EndTime.ToLocalTime();
                        response.WeekTable.Add(weektable);
                    }
            }

            return response;
        }
        /// <summary>
        /// 取得個人的傳統課表資訊
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public WeekTableViewModel GetByToken(string token)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token);
            if (tokenInfo == null)
                return null;

            var learningCircleService = new LearningCircleService();
            var learningCircles = learningCircleService.GetLearningCircleListByToken(token, null, null);
            if (learningCircles == null)
                return null;

            var responseData = new WeekTableViewModel();
            foreach (var learningCircle in learningCircles)
            {
                var data = GetByLearningCircleId(learningCircle.Id);
                responseData.WeekTable = new List<WeekTable>();
                if (data != null)
                    responseData.WeekTable.AddRange(data);
                responseData.StartDate = learningCircle.StartDate.Value.ToLocalTime();
                responseData.EndDate = learningCircle.EndDate.Value.ToLocalTime();
            }
            return responseData;
        }

        /// <summary>
        /// 取得課時間地點
        /// </summary>
        /// <param name="learningcircleId"></param>
        /// <returns></returns>
        public IEnumerable<WeekTable> GetByLearningCircleId(int learningcircleId)
        {
            var db = _uow.DbContext;
            var datas = db.WeekTable.Where(t => t.LearningCircleId == learningcircleId);
            var response = datas.ToList();
            if (response.FirstOrDefault() == null)
                return null;
            return response;
        }
        /// <summary>
        /// 取得傳統周課表
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable> GetAllMyCourseWeekTableData(int learningCircleId)
        {
            var datas = GetByLearningCircleId(learningCircleId);
            var response = new List<Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable>();
            if (datas == null)
                return null;
            foreach (var data in datas)
            {
                var responseData = new Infrastructure.ViewModel.CourseManage.GetAllMyCourseWeekTable()
                {
                    EndPeriod = data.EndPeriod.HasValue ? data.EndPeriod.Value : 0,
                    StartPeriod = data.StartPeriod.HasValue ? data.StartPeriod.Value : 0,
                    EndTime = string.Format("{0:HH}:{1:mm}", data.EndTime.ToLocalTime(), data.EndTime.ToLocalTime()),
                    StartTime = string.Format("{0:HH}:{1:mm}", data.StartTime.ToLocalTime(), data.StartTime.ToLocalTime()),
                    Place = data.Place,
                    Week = data.Week
                };
                response.Add(responseData);
            }
            return response;
        }

        /// <summary>
        /// 建立多筆傳統周課表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="classWeekType"></param>
        /// <param name="place"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="weeks"></param>
        /// <returns></returns>
        public IEnumerable<WeekTable> CreateWeekDatas(string token, string circleKey, int classWeekType, string place, DateTime startDate, DateTime endDate, List<int> weeks)
        {
            var saveDatas = new List<WeekTable>();
            if (weeks.Count <= 0) //只新增一筆
            {
                var data = CreateWeekData(token, circleKey, classWeekType, place, startDate, endDate);
                if (data == null)
                    return null;
                saveDatas.Add(data);
                return saveDatas;
            }

            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return null;

            var learningCircleService = new LearningCircleService();
            var learningCircleinfo = learningCircleService.GetDetailByOuterKey(circleKey);
            if (learningCircleinfo == null)
                return null;

            var startHour = Convert.ToInt32(startDate.ToString("HH"));
            var endHour = Convert.ToInt32(endDate.ToString("HH"));
            var dayOfWeekService = new Utility.DayOfWeekTools();
            var startWeek = startDate.DayOfWeek;
            //9點以前都是第一節，9點以後都(目前小時-7)節
            startHour = startHour < 9 ? 1 : startHour - 7;
            endHour = endHour < 9 ? 1 : endHour - 7;

            try
            {
                var db = _uow.DbContext;
                weeks = weeks.OrderBy(t => t).ToList();
                var weekMax = weeks.Max(t => t);
                if (weekMax >= 7)
                    return null;

                foreach (var week in weeks)
                {
                    var weekInfo = (DayOfWeek)week;
                    var _startDate = startDate;
                    var checkWeek = week - (int)startWeek;
                    if (checkWeek < 0)
                        _startDate = startDate.AddDays(7 + checkWeek);
                    else if (checkWeek > 0)
                        _startDate = startDate.AddDays(checkWeek);
                    var entity = new WeekTable()
                    {
                        ClassWeekType = classWeekType,
                        CreateUtcDate = DateTime.UtcNow,
                        Creator = tokenInfo.MemberId,
                        EndPeriod = endHour,
                        EndTime = endDate.ToUniversalTime(),
                        LearningCircleId = learningCircleinfo.Id,
                        Place = place,
                        StartPeriod = startHour,
                        StartTime = _startDate.ToUniversalTime(),
                        Week = dayOfWeekService.ChangeToCht(weekInfo)
                    };
                    saveDatas.Add(entity);
                }
                db.WeekTable.AddRange(saveDatas);
                db.SaveChanges();
                return saveDatas;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 建立傳統周課表 - 一筆傳統周課表
        /// </summary>
        /// <returns></returns>
        public WeekTable CreateWeekData(string token, string circleKey, int classWeekType, string place, DateTime startDate, DateTime endDate)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return null;

            var learningCircleService = new LearningCircleService();
            var learningCircleinfo = learningCircleService.GetDetailByOuterKey(circleKey);
            if (learningCircleinfo == null)
                return null;
            var startHour = Convert.ToInt32(startDate.ToString("HH"));
            var endHour = Convert.ToInt32(endDate.ToString("HH"));
            var dayOfWeekService = new Utility.DayOfWeekTools();
            var week = dayOfWeekService.ChangeToCht(startDate.DayOfWeek);
            //9點以前都是第一節，9點以後都(目前小時-7)節
            startHour = startHour <= 9 ? 1 : startHour - 7;
            endHour = endHour <= 9 ? 1 : endHour - 7;
            var entity = new WeekTable()
            {
                ClassWeekType = classWeekType,
                CreateUtcDate = DateTime.UtcNow,
                Creator = tokenInfo.MemberId,
                EndPeriod = endHour,
                EndTime = endDate.ToUniversalTime(),
                LearningCircleId = learningCircleinfo.Id,
                Place = place,
                StartPeriod = startHour,
                StartTime = startDate.ToUniversalTime(),
                Week = week
            };
            try
            {
                var db = _uow.DbContext;
                db.WeekTable.Add(entity);
                db.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 修改周課表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="place"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="weeks"></param>
        /// <returns></returns>
        public bool UpdateWeekDatas(string token, string circleKey, string place, DateTime startDate, DateTime endDate, int classWeekType, List<int> weeks)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return false;

            var learningCircleService = new LearningCircleService();
            var learningCircleinfo = learningCircleService.GetDetailByOuterKey(circleKey);
            if (learningCircleinfo == null)
                return false;

            var startHour = Convert.ToInt32(startDate.ToString("HH"));
            var endHour = Convert.ToInt32(endDate.ToString("HH"));
            var dayOfWeekService = new Utility.DayOfWeekTools();
            var startWeek = startDate.DayOfWeek;
            //9點以前都是第一節，9點以後都(目前小時-7)節
            startHour = startHour <= 9 ? 1 : startHour - 7;
            endHour = endHour <= 9 ? 1 : endHour - 7;

            try
            {
                var db = _uow.DbContext;
                //排序 周日:0 ~周六:6
                weeks = weeks.OrderBy(t => t).ToList();
                //查看week是否有錯誤資料
                var weekMax = weeks.Max(t => t);
                if (weekMax >= 7)
                    return false;
                //取得資料庫目前傳統課表
                var sqlWeekTablesDatas = GetByLearningCircleId(learningCircleinfo.Id);
                //暫存作為刪除的資料
                var tempWeekTableDatas = sqlWeekTablesDatas.ToList();
                foreach (var week in weeks)
                {
                    var weekInfo = (DayOfWeek)week;
                    //踢出不要刪除的weekTable資訊
                    if (tempWeekTableDatas.Count() > 0)
                    {
                        var removeTempData = tempWeekTableDatas.FirstOrDefault(t => t.StartTime.ToLocalTime().DayOfWeek.ToString() == weekInfo.ToString());
                        if (removeTempData != null)
                            tempWeekTableDatas.Remove(removeTempData);
                    }

                    //設定開始日期
                    var _startDate = startDate;
                    var checkWeek = week - (int)startWeek;
                    if (checkWeek < 0)
                        _startDate = startDate.AddDays(7 + checkWeek);
                    else if (checkWeek > 0)
                        _startDate = startDate.AddDays(checkWeek);

                    //找出並修改要保留的資料
                    var editDataInfo = sqlWeekTablesDatas.FirstOrDefault(t => t.StartTime.ToLocalTime().DayOfWeek.ToString() == weekInfo.ToString());
                    if (editDataInfo != null)
                    {
                        editDataInfo.Place = place;
                        editDataInfo.StartTime = _startDate.ToUniversalTime();
                        editDataInfo.EndTime = endDate.ToUniversalTime();
                        editDataInfo.StartPeriod = startHour;
                        editDataInfo.EndPeriod = endHour;
                        editDataInfo.ClassWeekType = classWeekType;
                        continue;
                    }

                    //資料庫無該資料，需要新增
                    var entity = new WeekTable()
                    {
                        ClassWeekType = classWeekType,
                        CreateUtcDate = DateTime.UtcNow,
                        Creator = tokenInfo.MemberId,
                        EndPeriod = endHour,
                        EndTime = endDate.ToUniversalTime(),
                        LearningCircleId = learningCircleinfo.Id,
                        Place = place,
                        StartPeriod = startHour,
                        StartTime = _startDate.ToUniversalTime(),
                        Week = dayOfWeekService.ChangeToCht(weekInfo)
                    };
                    db.WeekTable.Add(entity);

                }
                //刪除timeTable資訊
                foreach (var deleteWeek in tempWeekTableDatas)
                {
                    var timeTableInfo = db.TimeTable.Where(t => t.Course_No == circleKey).ToList();
                    var deleteTimeTableDatas = timeTableInfo.Where(t => t.StartDate.Value.DayOfWeek.ToString() == deleteWeek.StartTime.DayOfWeek.ToString()).ToList();
                    db.TimeTable.RemoveRange(deleteTimeTableDatas);
                }
                //刪除weektable資訊
                db.WeekTable.RemoveRange(tempWeekTableDatas);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 處理資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool WeekTableDataProxy(WeekTablePostRequest requestData)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null)
                return false;
            if (requestData.WeekTableData == null)
                return false;
            var insertDatas = requestData.WeekTableData.WeekTable.Where(t => t.Id == 0);
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(requestData.CircleKey.ToLower());
            if (learningCircleInfo == null)
                return false;
            var startDateTime = requestData.WeekTableData.StartDate;
            var endDateTime = requestData.WeekTableData.EndDate;
            var dayOfWeekTools = new Utility.DayOfWeekTools();
            var DateTimeTools = new Utility.DateTimeTools();
            //處理新增資料
            foreach (var insertData in insertDatas)
            {
                insertData.LearningCircleId = learningCircleInfo.Id;
                insertData.ClassWeekType = 0;
                insertData.Creator = tokenInfo.MemberId;
                insertData.CreateUtcDate = DateTime.UtcNow;
                var insertStartPeriod = DateTimeTools.DatetimeToSectionint(insertData.StartTime, true);
                var insertEndPeriod = DateTimeTools.DatetimeToSectionint(insertData.EndTime, false);
                insertData.StartPeriod = insertStartPeriod;
                insertData.EndPeriod = insertEndPeriod;
                /*判斷week字串與startTime的時與分，再組成對的時間*/
                //取出時與分
                var insertStartHour = insertData.StartTime.ToUniversalTime().Hour;
                var insertStartMinute = insertData.StartTime.ToUniversalTime().Minute;
                var insertEndHour = insertData.EndTime.ToUniversalTime().Hour;
                var insertEndMinute = insertData.EndTime.ToUniversalTime().Minute;
                var insertDayOfWeek = dayOfWeekTools.ChangeToDayOfWeek(insertData.Week);
                if (insertDayOfWeek == null)
                    return false;
                var startDayOfWeek = startDateTime.DayOfWeek;
                if (insertDayOfWeek.Value != startDayOfWeek)
                {
                    var interval = (int)startDayOfWeek - (int)insertDayOfWeek.Value;
                    if (interval > 0)
                        startDateTime = startDateTime.AddDays(interval);
                    else
                        startDateTime = startDateTime.AddDays(7 + interval);
                }
                startDateTime = startDateTime.AddHours(insertStartHour);
                startDateTime = startDateTime.AddMinutes(insertStartMinute);
                endDateTime = endDateTime.AddHours(insertEndHour);
                endDateTime = endDateTime.AddMinutes(insertEndMinute);
                insertData.StartTime = startDateTime.ToUniversalTime();
                insertData.EndTime = endDateTime.ToUniversalTime();
            }
            try
            {
                var db = _uow.DbContext;
                var updateLearningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == requestData.CircleKey.ToLower());
                if (updateLearningCircleInfo != null)
                {
                    updateLearningCircleInfo.StartDate = requestData.WeekTableData.StartDate.ToUniversalTime();
                    updateLearningCircleInfo.EndDate = requestData.WeekTableData.EndDate.ToUniversalTime();
                }
                db.WeekTable.AddRange(insertDatas);
                var dbData = GetByCirclekey(requestData.CircleKey.ToLower());
                //聯集資料
                var unionDataIds = requestData.WeekTableData.WeekTable.Select(t => t.Id).Union(dbData.WeekTable.Select(t => t.Id));
                //找出欲刪除的資料
                var deleteDataIds = unionDataIds.Except(requestData.WeekTableData.WeekTable.Select(t => t.Id));
                foreach (var deleteDataId in deleteDataIds)
                {
                    var deleteTimeTable = db.WeekTable.Find(deleteDataId);
                    if (deleteTimeTable == null)
                        continue;
                    db.WeekTable.Remove(deleteTimeTable);
                }
                //處理編輯資料
                var updateDatas = requestData.WeekTableData.WeekTable.Where(t => t.Id > 0).ToList();
                startDateTime = requestData.WeekTableData.StartDate;
                endDateTime = requestData.WeekTableData.EndDate;
                foreach (var updateData in updateDatas)
                {
                    var updateDataInfo = db.WeekTable.Find(updateData.Id);
                    var updateStartPeriod = DateTimeTools.DatetimeToSectionint(updateData.StartTime, true);
                    var updateEndPeriod = DateTimeTools.DatetimeToSectionint(updateData.EndTime, false);
                    updateDataInfo.StartPeriod = updateStartPeriod;
                    updateDataInfo.EndPeriod = updateEndPeriod;
                    updateDataInfo.UpdateUtcDate = DateTime.UtcNow;
                    updateDataInfo.Updater = tokenInfo.MemberId;
                    /*判斷week字串與startTime的時與分，再組成對的時間*/
                    //取出時與分
                    var updateStartHour = updateData.StartTime.ToUniversalTime().Hour;
                    var updateStartMinute = updateData.StartTime.ToUniversalTime().Minute;
                    var updateEndHour = updateData.EndTime.ToUniversalTime().Hour;
                    var updateEndMinute = updateData.EndTime.ToUniversalTime().Minute;
                    var updateDayOfWeek = dayOfWeekTools.ChangeToDayOfWeek(updateData.Week);
                    if (updateDayOfWeek == null)
                        return false;
                    var startDayOfWeek = startDateTime.DayOfWeek;
                    if (updateDayOfWeek.Value != startDayOfWeek)
                    {
                        var interval = (int)startDayOfWeek - (int)updateDayOfWeek.Value;
                        if (interval > 0)
                            startDateTime = startDateTime.AddDays(interval);
                        else
                            startDateTime = startDateTime.AddDays(7 + interval);
                    }
                    startDateTime = startDateTime.AddHours(updateStartHour);
                    startDateTime = startDateTime.AddMinutes(updateStartMinute);
                    endDateTime = endDateTime.AddHours(updateEndHour);
                    endDateTime = endDateTime.AddMinutes(updateEndMinute);
                    updateDataInfo.StartTime = updateData.StartTime.ToUniversalTime();
                    updateDataInfo.EndTime = updateData.EndTime.ToUniversalTime();
                    updateDataInfo.Week = updateData.Week;
                    updateDataInfo.Place = updateData.Place;

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

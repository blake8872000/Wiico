using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Utility
{
    public class DateTimeTools
    {

        /// <summary>
        /// 取得範圍內的所有間隔時間
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="interval">設定間隔天數</param>
        /// <param name="circleKey">為了顯示傳統課表資訊</param>
        /// <returns></returns>
        public List<Infrastructure.ViewModel.CourseManage.GetWeekBySemeResponse> GetIntervalDateList(DateTime startDate, DateTime endDate, int interval)
        {
            var responseData = new List<Infrastructure.ViewModel.CourseManage.GetWeekBySemeResponse>();
            var startWeek = startDate.DayOfYear / 7; //計算起始日為一年中的第幾周
            var endWeek = endDate.DayOfYear / 7; //計算結束日為一年中的第幾周
            if (startWeek <= 0)
                startWeek = 1;
            var startWeekDay = (int)startDate.DayOfWeek;
            var weekCount = (endWeek - startWeek + 1) > 0 ? (endWeek - startWeek + 1) : 52 - startWeek + endWeek; //計算間隔總共有幾周
            for (var week = 0; week <= weekCount; week++)
            {
                var _startDate = startDate.AddDays(week * interval); //根據設定的間隔天數來計算
                var _endDate = startDate.AddDays((week + 1) * interval).Date.AddHours(endDate.Hour).AddMinutes(endDate.Minute);//根據設定的間隔天數來計算
                var _data = new Infrastructure.ViewModel.CourseManage.GetWeekBySemeResponse()
                {
                    End_date = _endDate,
                    Start_date = _startDate
                };
                responseData.Add(_data);
            }
            return responseData;
        }

        /// <summary>
        /// 處理timeTable資料
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="circleKey"></param>
        /// <param name="classWeekType"></param>
        /// <param name="weekTables"></param>
        /// <returns></returns>
        public List<TimeTable> Getdata(DateTime startDate, DateTime endDate, string circleKey, int? classWeekType = 0, List<WeekTable> weekTables = null)
        {
            var responseData = new List<Infrastructure.Entity.TimeTable>();
            var startWeekDay = (int)startDate.DayOfWeek;
            //間隔週數
            var weekCount = Math.Ceiling(((Double)(endDate - startDate).Days + 1) / (Double)7);

            //查詢傳統周課表
            if (circleKey != null && circleKey != string.Empty) //要取得傳統課表資料
            {
                var weekTableService = new WeekTableService();
                if (weekTables == null)
                    weekTables = weekTableService.GetByCirclekey(circleKey).WeekTable.OrderBy(t => t.StartTime).ToList(); //遞增排序

                if (weekTables == null)
                    return null;
                var learningCircleService = new LearningCircleService();
                var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey);

                if (learningCircleInfo == null)
                    return null;
                var dayOfWeekTools = new DayOfWeekTools();
                var weeks = weekTables.Select(t => dayOfWeekTools.ChangeToDayOfWeek(t.Week).Value).OrderBy(t => t);
                if (weeks == null)
                    return null;
                var weeksMaxValue = Convert.ToInt32(weeks.Max(t => t)); //取出周次陣列中最大的數字
                var weeksMinValue = Convert.ToInt32(weeks.Min(t => t)); //取出周次陣列中最小的數字
                if (weeksMaxValue >= 7)//周次大於等於7不合邏輯，直接跳null
                    return null;

                var _startDate = startDate.Date;
                var _endDate = startDate.Date;
                if (classWeekType.Value == 0)
                {
                    //每周
                    for (var week = 0; week <= weekCount; week++)
                    {
                        #region 設定第一周日期區間
                        if (week == 0)
                        {
                            var getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                            responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));

                            foreach (var weekDay in weeks)
                            {
                                var checkWeek = (int)weekDay - (int)_startDate.DayOfWeek;
                                if (checkWeek > 0)
                                {
                                    _startDate = _startDate.AddDays(checkWeek);
                                    _endDate = _endDate.AddDays(checkWeek);
                                    getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                                    responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));
                                }
                            }
                        }
                        #endregion
                        #region 設定第二周到最終週日期區間
                        else
                        {
                            //第二周開始
                            //設定起始日期

                            foreach (var weekDay in weeks.GroupBy(t => t))
                            {
                                var checkWeek = (int)weekDay.Key - (int)_startDate.DayOfWeek;
                                if (checkWeek < 0)
                                {
                                    _endDate = _endDate.AddDays((7 + checkWeek));
                                    _startDate = _startDate.AddDays((7 + checkWeek));
                                }
                                else if (checkWeek == 0)
                                {
                                    _endDate = _endDate.AddDays(7);
                                    _startDate = _startDate.AddDays(7);
                                }
                                else
                                {
                                    _endDate = _endDate.AddDays(checkWeek);
                                    _startDate = _startDate.AddDays(checkWeek);
                                }
                                if (_startDate.Date <= endDate.Date)
                                {
                                    var getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                                    responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));

                                }
                            }
                        }
                        #endregion
                    }
                }
                //隔週
                else if (classWeekType.Value == 2)
                {
                    for (var week = 0; week <= weekCount; week += 2)
                    {
                        #region 設定第一周日期區間
                        if (week == 0)
                        {
                            var getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                            responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));

                            foreach (var weekDay in weeks)
                            {
                                var checkWeek = (int)weekDay - (int)_startDate.DayOfWeek;
                                if (checkWeek > 0)
                                {

                                    _startDate = _startDate.AddDays(checkWeek);
                                    _endDate = _endDate.AddDays(checkWeek);
                                    getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                                    responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));

                                }
                            }
                        }
                        #endregion
                        #region 設定第二周到最終週日期區間
                        else
                        {
                            //第二周開始
                            //設定起始日期

                            foreach (var weekDay in weeks.GroupBy(t => t))
                            {
                                var checkWeek = (int)weekDay.Key - (int)_startDate.DayOfWeek;
                                if (checkWeek < 0)
                                {
                                    _endDate = _endDate.AddDays((14 + checkWeek));
                                    _startDate = _startDate.AddDays((14 + checkWeek));
                                }
                                else if (checkWeek == 0)
                                {
                                    _endDate = _endDate.AddDays(14);
                                    _startDate = _startDate.AddDays(14);
                                }
                                else
                                {
                                    _endDate = _endDate.AddDays(checkWeek);
                                    _startDate = _startDate.AddDays(checkWeek);
                                }
                                if (_startDate.Date <= endDate.Date)
                                {
                                    var getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                                    responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    var getTimeSpan = weekTables.Where(t => (int)dayOfWeekTools.ChangeToDayOfWeek(t.Week) == (int)_startDate.DayOfWeek).ToList();
                    responseData.AddRange(SwitchWeekTableToTimeTable(getTimeSpan, learningCircleInfo.LearningOuterKey.ToLower(), _startDate, _endDate));
                }
            }
            return responseData;
        }

        /// <summary>
        /// 根據WeekTable資料重新計算展開的TimeTable資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<TimeTable> GetTimeTableByWeekTable(WeekTablePostRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token).Result;
            if (checkToken == null)
                return null;
            var datas = Getdata(requestData.WeekTableData.StartDate, requestData.WeekTableData.EndDate, requestData.CircleKey.ToLower(), 0, requestData.WeekTableData.WeekTable);
            foreach (var data in datas)
            {
                if (data.StartDate.HasValue)
                {
                    DateTime? _startDate = data.StartDate.Value.ToLocalTime();
                    data.StartDate = _startDate;
                }
                if (data.EndDate.HasValue)
                {
                    DateTime? _endDate = data.EndDate.Value.ToLocalTime();
                    data.EndDate = _endDate;
                }

            }
            return datas.OrderBy(t => t.StartDate).ToList();
        }

        public int DatetimeToSectionint(DateTime date, bool isStart)
        {
            var sectionHour = date.Hour;
            switch (sectionHour)
            {
                case 8:
                    return 1;
                case 9:
                    if (isStart)
                        return 2;
                    else
                        return 1;
                case 10:
                    if (isStart)
                        return 3;
                    else
                        return 2;
                case 11:
                    if (isStart)
                        return 4;
                    else
                        return 3;
                case 12:
                    if (isStart)
                        return 5;
                    else
                        return 4;
                case 13:
                    if (isStart)
                        return 6;
                    else
                        return 5;
                case 14:
                    if (isStart)
                        return 7;
                    else
                        return 6;
                case 15:
                    if (isStart)
                        return 8;
                    else
                        return 7;
                case 16:
                    if (isStart)
                        return 9;
                    else
                        return 8;
                case 17:
                    if (isStart)
                        return 10;
                    else
                        return 9;
                case 18:
                    if (isStart)
                        return 11;
                    else
                        return 10;
                case 19:
                    if (isStart)
                        return 12;
                    else
                        return 11;
                case 20:
                    if (isStart)
                        return 13;
                    else
                        return 12;
                case 21:
                    if (isStart)
                        return 14;
                    else
                        return 13;
                case 22:
                    if (isStart)
                        return 15;
                    else
                        return 14;
                case 23:
                    if (isStart)
                        return 16;
                    else
                        return 15;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// 處理節次日期(開始結束時間不同)
        /// </summary>
        /// <param name="date">欲顯示時間</param>
        /// <param name="section">設定節次</param>
        /// <param name="isStart">是否為開始</param>
        /// <returns></returns>
        public DateTime ProxyDateTime(DateTime date, int section, bool isStart)
        {
            switch (section)
            {
                case 1:
                    if (isStart)
                    {
                        date = date.AddHours(8);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(9);

                    break;
                case 2:
                    if (isStart)
                    {
                        date = date.AddHours(9);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(10);

                    break;
                case 3:
                    if (isStart)
                    {
                        date = date.AddHours(10);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(11);

                    break;
                case 4:
                    if (isStart)
                    {
                        date = date.AddHours(11);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(12);

                    break;
                case 5:
                    if (isStart)
                    {
                        date = date.AddHours(12);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(13);

                    break;
                case 6:
                    if (isStart)
                    {
                        date = date.AddHours(13);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(14);

                    break;
                case 7:
                    if (isStart)
                    {
                        date = date.AddHours(14);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(15);

                    break;
                case 8:
                    if (isStart)
                    {
                        date = date.AddHours(15);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(16);

                    break;
                case 9:
                    if (isStart)
                    {
                        date = date.AddHours(16);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(17);

                    break;
                case 10:
                    if (isStart)
                    {
                        date = date.AddHours(17);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(18);

                    break;
                case 11:
                    if (isStart)
                    {
                        date = date.AddHours(18);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(19);

                    break;
                case 12:
                    if (isStart)
                    {
                        date = date.AddHours(19);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(20);

                    break;
                case 13:
                    if (isStart)
                    {
                        date = date.AddHours(20);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(21);

                    break;
                case 14:
                    if (isStart)
                    {
                        date = date.AddHours(21);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(22);

                    break;
                case 15:
                    if (isStart)
                    {
                        date = date.AddHours(22);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(23);

                    break;
                case 16:
                    if (isStart)
                    {
                        date = date.AddHours(23);
                        date = date.AddMinutes(10);
                    }
                    else
                        date = date.AddHours(24);

                    break;
                default:
                    break;
            }
            return date;
        }


        private List<TimeTable> SwitchWeekTableToTimeTable(List<WeekTable> datas, string circleKey, DateTime _startDate, DateTime _endDate)
        {
            var responseData = new List<TimeTable>();
            foreach (var timespan in datas)
            {
                var startTimeSpan = timespan.StartTime.Kind == DateTimeKind.Utc ?
                                                            (_endDate + timespan.StartTime.TimeOfDay) :
                                                            (_endDate + timespan.StartTime.TimeOfDay).ToUniversalTime();
                var endTimeSpan = timespan.EndTime.Kind == DateTimeKind.Utc ?
                                             (_endDate + timespan.EndTime.TimeOfDay) :
                                             (_endDate + timespan.EndTime.TimeOfDay).ToUniversalTime();
                var _data = new Infrastructure.Entity.TimeTable()
                {
                    EndDate = endTimeSpan,
                    StartDate = startTimeSpan,
                    ClassRoom = timespan.Place,
                    Course_No = circleKey
                };
                responseData.Add(_data);
            }
            return responseData;
        }
    }
}

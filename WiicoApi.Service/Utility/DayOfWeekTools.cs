using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Service.Utility
{
    public class DayOfWeekTools
    {
        public string ChangeToCht(DayOfWeek week)
        {
            switch (week)
            {
                case DayOfWeek.Monday: return "一";
                case DayOfWeek.Tuesday: return "二";
                case DayOfWeek.Wednesday: return "三";
                case DayOfWeek.Thursday: return "四";
                case DayOfWeek.Friday: return "五";
                case DayOfWeek.Saturday: return "六";
                case DayOfWeek.Sunday: return "日";
                default: return "無法判斷星期";
            }
        }
        public DayOfWeek? ChangeToDayOfWeek(string week)
        {
            switch (week)
            {
                case "一": return DayOfWeek.Monday;
                case "二": return DayOfWeek.Tuesday;
                case "三": return DayOfWeek.Wednesday;
                case "四": return DayOfWeek.Thursday;
                case "五": return DayOfWeek.Friday;
                case "六": return DayOfWeek.Saturday;
                case "日": return DayOfWeek.Sunday;
                default: return null;
            }
        }
    }
}
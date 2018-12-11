using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetMyCourseScheduleResponse
    {
        public int Id { get; set; }
        /// <summary>
        /// 課表重複類型
        /// </summary>
        public enum enumClassWeekType : int
        {
            /// <summary>
            /// 每周上課
            /// </summary>
            EveryWeek = 0,
            /// <summary>
            /// 單周上課
            /// </summary>
            SingleWeek = 1,
            /// <summary>
            /// 雙周上課
            /// </summary>
            DoubleWeek = 2
        }

        /// <summary>
        /// 教室編號(eg:01-307), beacon對照表使用
        /// </summary>
        public string RoomID { get; set; }
        /// <summary>
        /// 教室中文名稱(eg:建-307) ,,,,,,, 需要確認輸出內容方式
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// 課程代碼
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 課程開始堂號
        /// </summary>
        public int StartPeriod { get; set; }
        /// <summary>
        /// 課程結束堂號
        /// </summary>
        public int EndPeriod { get; set; }
        /// <summary>
        /// 課程開始時間(對應開始堂次)
        /// </summary>
        public string StartPeriodTime { get; set; }
        /// <summary>
        /// 課程結束時間(對應結束堂次)
        /// </summary>
        public string EndPeriodTime { get; set; }
        /// <summary>
        /// 課程於學期的開始時間
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 課程於學期的結束時間
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 課表重複類型
        /// </summary>
        public enumClassWeekType ClassWeekType { get; set; }
        /// <summary>
        /// 課程星期代碼(星期日=0,星期一=1...星期六=6)
        /// </summary>
        public DayOfWeek WeekDay { get; set; }
        /// <summary>
        /// 星期幾的對應中文名稱
        /// </summary>
        public string NameOfWeekDay { get; set; }
        /// <summary>
        /// beacon設備的UUID
        /// </summary>
        public string BeaconUUID { get; set; }
        /// <summary>
        /// beacon設備的Major號碼
        /// </summary>
        public string BeaconMajor { get; set; }
        /// <summary>
        /// beacon設備的Minor號碼
        /// </summary>
        public string BeaconMinor { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetAllMyCourseResponse
    {
        /// <summary>
        /// 課程所屬年度
        /// </summary>
      //  [JsonProperty("year")]
        public int Year { get; set; }
        /// <summary>
        /// 學期名稱
        /// </summary>
       // [JsonProperty("semeName")]
        public string SemeName { get; set; }
        /// <summary>
        /// 課程所屬年度加上學期代碼(eg:1033)
        /// </summary>
       // [JsonProperty("yearSeme")]
        public string YearSeme { get; set; }
        /// <summary>
        /// 是否為本學期課程
        /// </summary>
      //  [JsonProperty("isNowSeme")]
        public bool IsNowSeme { get; set; }
        /// <summary>
        /// 尚未讀取的公告數量以及參考資料的總數
        /// </summary>
      //  [JsonProperty("unreadAlert")]
        public int UnreadAlert { get; set; }
        /// <summary>
        /// 課程清單
        /// </summary>
       // [JsonProperty("course")]
        public DataCourseDataModel[] Course { get; set; }
    }
}

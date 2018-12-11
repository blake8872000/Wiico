using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetCourseSyllabusResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 課程大綱(進度)ID
        /// </summary>
        [JsonProperty("syll_id")]
        public string Syll_id { get; set; }
        /// <summary>
        /// 課程大綱(進度)排序
        /// </summary>
        [JsonProperty("syllSort")]
        public int SyllSort { get; set; }
        /// <summary>
        /// 課程大綱(進度)名稱
        /// </summary>
        [JsonProperty("syllTitle")]
        public string SyllTitle { get; set; }
        [JsonProperty("syllNote")]
        public string SyllNote { get; set; }
        /// <summary>
        /// 課程大綱(進度)時間
        /// </summary>
        [JsonProperty("syll_date")]
        public DateTime? Syll_date { get; set; }
        /// <summary>
        /// 活動列表
        /// </summary>
        [JsonProperty("activityList")]
        public IEnumerable<ActivitySyllabusResponseData> ActivityList{ get; set; }
    }
}

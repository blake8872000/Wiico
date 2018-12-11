using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 課周次表
    /// </summary>
    public class WeekTable
    {
        /// <summary>
        /// 課周次表編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 第幾周
        /// </summary>
        [JsonProperty("week")]
        public string Week { get; set; }
        /// <summary>
        /// 節次
        /// </summary>
        [JsonIgnore]
        public int? StartPeriod { get; set; }
        /// <summary>
        /// 節次
        /// </summary>
        [JsonIgnore]
        public int? EndPeriod { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 上課地點
        /// </summary>
        [JsonProperty("place")]
        public string Place { get; set; }

        /// <summary>
        /// 上課地點編號
        /// </summary>
        [JsonProperty("classRoomId")]
        public string ClassRoomId { get; set; }

        /// <summary>
        ///  學習圈編號
        /// </summary>
        [JsonIgnore]
        public int LearningCircleId { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        [JsonIgnore]
        public DateTime CreateUtcDate { get; set; }
        /// <summary>
        /// 建立者編號
        /// </summary>
        [JsonIgnore]
        public int Creator { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [JsonIgnore]
        public DateTime? UpdateUtcDate { get; set; }
        /// <summary>
        /// 更新者編號
        /// </summary>
        [JsonIgnore]
        public int? Updater { get; set; }
        /// <summary>
        /// 上課時間類型 0 : 每周上課 | 1 : 單次上課 | 2 : 雙周上課
        /// </summary>
        [JsonProperty("classWeekType")]
        public int ClassWeekType { get; set; }
    }
}

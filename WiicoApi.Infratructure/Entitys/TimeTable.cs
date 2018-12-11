using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class TimeTable
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 課程編號
        /// </summary>
        [JsonIgnore]
        public int? Course_Id { get; set; }

        /// <summary>
        /// 教室代碼
        /// </summary>
        [JsonIgnore]
        public string ClassRoomId { get; set; }

        /// <summary>
        /// 上課地點 - 其他地點
        /// </summary>
        [JsonProperty("roomName")]
        public string ClassRoom { get; set; }
        /// <summary>
        /// 上課日期 縮寫
        /// </summary>
        [JsonIgnore]
        public string ClassTime { get; set; }
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonIgnore]
        public string Course_No { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startPeriodTime")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endPeriodTime")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 舊教室教室代碼
        /// </summary>
        [JsonIgnore]
        public string OriginClassRoomID { get; set; }
        /// <summary>
        /// 舊教室上課名稱
        /// </summary>
        [JsonIgnore]
        public string OriginClassRoomName { get; set; }

        /// <summary>
        /// 舊教室上課開始日期
        /// </summary>
        [JsonIgnore]
        public DateTime? OriginStartTime { get; set; }

        /// <summary>
        /// 舊教室上課結束日期
        /// </summary>
        [JsonIgnore]
        public DateTime? OriginEndTime { get; set; }

        /// <summary>
        /// 異動原因
        /// </summary>
        [JsonIgnore]
        public int? ChangeReason { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [JsonProperty("reMark")]
        public string Remark { get; set; }
    }
}

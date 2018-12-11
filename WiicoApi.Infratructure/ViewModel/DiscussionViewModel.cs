using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 主題討論輸出資訊
    /// </summary>
    public class DiscussionViewModel : ValueObject.ActivityBase
    {
        /// <summary>
        /// 活動編號
        /// </summary>
        [JsonProperty("activityId")]
        public int ActivityId { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }

        /// <summary>
        /// 活動 Guid
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        /// <summary>
        /// 主題討論活動名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 建立者id
        /// </summary>
        [JsonIgnore]
        public string Creator { get; set; }


        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 發言人(登入者)的相片
        /// </summary>
        [JsonProperty("msgSenderPhoto")]
        public string MsgSenderPhoto { get; set; }

        /// <summary>
        /// 主題討論活動說明描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 主題討論空間
        /// </summary>
        [JsonProperty("googleDriveFolder")]
        public string GoogleDriveFolder { get; set; }

        /// <summary>
        /// 老師上傳的參考檔案總數
        /// </summary>
        [JsonProperty("fileCount")]
        public int FileCount { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 留言資訊
        /// </summary>
        [JsonProperty("msg")]
        public List<BusinessObject.MsgContent> Msg { get; set; }
        /// <summary>
        /// 主題討論點讚資訊
        /// </summary>
        [JsonProperty("like")]
        public BusinessObject.LikeCount Like { get; set; }
        /// <summary>
        /// 要顯示的頁數
        /// </summary>
        public int? Pages { get; set; }
        /// <summary>
        /// 要顯示的數量
        /// </summary>
        public int? Rows { get; set; }
    }
}

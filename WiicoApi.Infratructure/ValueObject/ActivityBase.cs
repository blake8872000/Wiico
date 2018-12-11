using WiicoApi.Infrastructure.Property;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class ActivityBase
    {
        /// <summary>
        /// 活動 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 學習圈(群組)代碼
        /// </summary>
        [JsonProperty("groupId")]
        private string _toRoomId;
        public string ToRoomId { get { return _toRoomId?.ToLower(); } set { _toRoomId = value; } }

        /// <summary>
        /// 活動模組代碼
        /// </summary>
        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }

        /// <summary>
        /// 文字訊息
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonIgnore]
        public Guid OuterKey { get; set; }

        [JsonProperty("outerKey")]
        public string sOuterKey { get; set; }

        /// <summary>
        /// 建立者帳號
        /// </summary>
        [JsonProperty("creatorAccount")]
        public string CreatorAccount { get; set; }

        /// <summary>
        /// 建立者姓名
        /// </summary>
        [JsonProperty("creatorName")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 建立者照片
        /// </summary>
        [JsonProperty("creatorPhoto")]
        public string CreatorPhoto { get; set; }
        /// <summary>
        /// 建立時間 UTC
        /// </summary>
        [JsonIgnore]
        public DateTime Created_Utc { get; set; }

        /// <summary>
        /// 建立時間 Local Time
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime? CreateTime_Local { get { return Created_Utc.ToLocalTime(); } }

        /// <summary>
        /// 活動時間 UTC
        /// </summary>
        [JsonIgnore]
        public DateTime? ActivityDate { get; set; }


        /// <summary>
        /// 活動時間 UTC
        /// </summary>
        [JsonProperty("activityTime")]
        public DateTime? ActivityToLocalDate { get { if (ActivityDate != null) return ActivityDate.Value.ToLocalTime(); else return null; } }

        /// <summary>
        /// 最後更新時間 UTC
        /// </summary>
        [JsonIgnore]
        public DateTime? Updated_Utc { get; set; }

        /// <summary>
        /// 最後更新時間 Local Time
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime? UpdatedTime_Local
        {
            get
            {
                if (Updated_Utc.HasValue)
                    return Updated_Utc.Value.ToLocalTime();
                else
                    return null;
            }
        }

        /// <summary>
        /// 刪除時間 UTC
        /// </summary>
        [JsonIgnore]
        public DateTime? Deleted_Utc { get; set; }
        [JsonIgnore]
        public TimeData Deleted { get; set; }

        /// <summary>
        /// 是否已刪除
        /// </summary>
        [JsonProperty("isDelete")]
        public bool IsDelete { get { return Deleted_Utc.HasValue; } }

        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonIgnore]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? jStartDate
        {
            get
            {
                if (StartDate.HasValue)
                    return StartDate.Value.ToLocalTime();
                else
                    return null;
            }
        }

        /// <summary>
        /// 持續時間
        /// </summary>
        [JsonProperty("duration")]
        public int? Duration { get; set; }

        /// <summary>
        /// 剩餘秒數
        /// </summary>
        [JsonProperty("remainSeconds")]
        public double RemainSeconds
        {
            get
            {
                if (StartDate.HasValue && Duration.HasValue)
                {
                    var endDate = StartDate.Value.Add(new TimeSpan(0, 0, Duration.Value));
                    return (endDate - DateTime.UtcNow).TotalSeconds;
                }
                return 0;
            }
        }

        /// <summary>
        /// 活動是否是進行中
        /// </summary>
        [JsonProperty("actived")]
        public bool Actived { get { return RemainSeconds > 0; } }


        /// <summary>
        /// 發布日期
        /// </summary>
        [JsonIgnore]
        public DateTime? Publish_Utc { get; set; }

        /// <summary>
        /// 顯示發布日期
        /// </summary>
        [JsonProperty("publish_date")]
        public DateTime? Publish_Date
        {
            get
            {
                if (Publish_Utc.HasValue && Publish_Utc != null)
                    return Publish_Utc.Value.ToLocalTime();
                else
                    return null;
            }
        }

    }
}

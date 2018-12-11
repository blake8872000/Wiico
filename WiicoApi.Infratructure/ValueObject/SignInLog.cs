using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 每位成員出席資訊
    /// </summary>
    public class SignInLog
    {
        /// <summary>
        /// 點名時間
        /// </summary>
        [JsonProperty("time")]
        public DateTime? Time { get; set; }
        /// <summary>
        /// 學生編號
        /// </summary>
        [JsonProperty("stuId")]
        public int StuId { get; set; }
        /// <summary>
        /// 學生帳號
        /// </summary>
        [JsonProperty("studId")]
        public string StudId { get; set; }
        /// <summary>
        /// 學生姓名
        /// </summary>
        [JsonProperty("studName")]
        public string StudName { get; set; }
        /// <summary>
        /// 學生頭像
        /// </summary>
        [JsonProperty("studPhoto")]
        public string StudPhoto { get; set; }
        /// <summary>
        /// 狀態名稱代碼
        /// </summary>
        [JsonIgnore]
        public string Status { get; set; }

        /// <summary>
        /// 請假代碼
        /// </summary>
        [JsonIgnore]
        public Guid? LeaveEventId { get; set; }

        /// <summary>
        /// 顯示請假代碼
        /// </summary>
        [JsonProperty("leaveOuterKey")]
        public string LeaveOuterKey { get; set; }

        /// <summary>
        /// 狀態編號
        /// </summary>
        [JsonProperty("status")]
        public int iStatus { get { return int.Parse(Status); } }
        /// <summary>
        /// 建立者帳號
        /// </summary>
        [JsonProperty("creatorAccount")]
        public string CreatorAccount { get; set; }
        /// <summary>
        /// 修改時間 -UTC時間
        /// </summary>
        [JsonIgnore]
        public DateTime? UpdateDate_Utc { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime? UpdateDate_Local
        {
            get
            {
                if (UpdateDate_Utc.HasValue)
                    return UpdateDate_Utc.Value.ToLocalTime();
                else
                    return null;

            }
        }
        /// <summary>
        /// 修改前狀態
        /// </summary>
        [JsonProperty("oldState")]
        public int OldState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("mode")]
        public int? Mode { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 請假狀態
        /// </summary>
        [JsonProperty("leaveStatus")]
        public string LeaveStatus { get; set; }
    }
}

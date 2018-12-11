using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    public class AttendanceLeave
    {
        /// <summary>
        /// 請假申請id - 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 非活動的事件代碼
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }

        /// <summary>
        /// 申請人MemId
        /// </summary>
        [JsonProperty("studId")]
        public int StudId { get; set; }

        /// <summary>
        /// 請假日期
        /// </summary>
        [JsonProperty("leaveDate")]
        public DateTime LeaveDate { get; set; }

        /// <summary>
        /// 申請假別:1 事假，2 病假
        /// </summary>
        //[MaxLength(2)]
        [JsonProperty("leaveType")]
        public int? LeaveType { get; set; }

        /// <summary>
        /// 申請主旨
        /// </summary>
        [MaxLength(200)]
        [JsonProperty("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 申請說明
        /// </summary>
        [MaxLength(500)]
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 申請狀態:00 已作廢,10 已完成,20 審核中,30 申請人抽回,40 已駁回
        /// </summary>
        [MaxLength(2)]
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// 審核批注
        /// </summary>
        [MaxLength(200)]
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// 建立者MemId
        /// </summary>
        [JsonProperty("creator")]
        public int Creator { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime? UpdateTime { get; set; }
    }
}

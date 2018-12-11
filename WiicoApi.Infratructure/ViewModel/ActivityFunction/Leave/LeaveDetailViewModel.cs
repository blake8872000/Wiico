using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    /// <summary>
    /// 請假單詳細資訊
    /// </summary>
    public class LeaveDetailViewModel
    {
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
        /// 申請狀態:00 已作廢,10 已完成,20 審核中,30 申請人抽回,40 已駁回
        /// </summary>
        [MaxLength(2)]
        [JsonProperty("statusName")]
        public string StatusName { get; set; }
        /// <summary>
        /// 審核批注
        /// </summary>
        [MaxLength(200)]
        [JsonProperty("comment")]
        public string Comment { get; set; }

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

        /// <summary>
        /// 是否為學生
        /// </summary>
        [JsonProperty("isManager")]
        public bool IsManager { get; set; }

        /// <summary>
        /// 請假單代碼
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }

        /// <summary>
        /// 申請人姓名
        /// </summary>
        [JsonProperty("memberName")]
        public string MemberName { get; set; }
        /// <summary>
        /// 申請人帳號
        /// </summary>

        [JsonProperty("memberAccount")]
        public string MemberAccount { get; set; }

        /// <summary>
        /// 申請人頭像
        /// </summary>
        [JsonProperty("memberImg")]
        public string MemberImg { get; set; }

        /// <summary>
        /// 課程名稱
        /// </summary>
        [JsonProperty("courseName")]
        public string CourseName { get; set; }

        /// <summary>
        /// 檔案清單
        /// </summary>
        [JsonProperty("fileList")]
        public List<Entity.GoogleFile> FileList { get; set; }
    }
}

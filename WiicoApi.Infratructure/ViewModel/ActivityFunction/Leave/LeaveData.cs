using WiicoApi.Infrastructure.ValueObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class LeaveData
    {
        /// <summary>
        /// 暫存請假單編號
        /// </summary>
        [JsonIgnore]
        public int LeaveId { get; set; }
        /// <summary>
        /// 是否有點名權限
        /// </summary>
        [JsonProperty("isManager")]
        public bool IsManager { get; set; }
        /// <summary>
        /// 請假單代碼
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        /// <summary>
        /// 請假人姓名
        /// </summary>
        [JsonProperty("memberName")]
        public string MemberName { get; set; }
        /// <summary>
        /// 請假人帳號(學號)
        /// </summary>
        [JsonProperty("memberAccount")]
        public string MemberAccount { get; set; }
        /// <summary>
        /// 狀態說明 ex:審核退單原因
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }
        /// <summary>
        /// 請假單狀態{"00", "已作廢"},{ "10", "已完成"},{ "20", "待審核"},{ "30", "已抽回"},{ "40", "已駁回"}
        /// </summary>
        [JsonProperty("status")]
        // public enumAbsenceFormStatus Status { get; set; 
        public string Status { get; set; }
        /// <summary>
        /// 請假原因
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
        /// <summary>
        /// 請假類別
        /// </summary>
        [JsonProperty("leaveType")]
        public enumLeaveType LeaveType { get; set; }
        /// <summary>
        /// 請假日期
        /// </summary>
        [JsonProperty("leaveDate")]
        public DateTime LeaveDate { get; set; }
        /// <summary>
        /// 送出假單日期
        /// </summary>
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 請假主旨
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; }
        /// <summary>
        /// 最後更新時間
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 請假單的附件檔案
        /// </summary>
        [JsonProperty("fileList")]
        public List<FileStorageViewModel>FileList { get; set; }
    }
}

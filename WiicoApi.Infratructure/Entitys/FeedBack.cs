using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 問題回報
    /// </summary>
    public class FeedBack
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 問題處理狀態 0 = 待處理 | 1 = 處理中 | 2 = 已處理 | 3 = 不處理  | 4 = 轉相關單位 
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        /// <summary>
        /// 問題回復
        /// </summary>
        [JsonProperty("reContent")]
        public string ReContent { get; set; }

        /// <summary>
        /// 問題類型 (問題回報 | 支持鼓勵 | 使用心得 | 系統錯誤 | 其他)
        /// </summary>
        [JsonProperty("feedBackType")]
        public string FeedBackType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// 回報裝置的系統型號
        /// </summary>
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("createTime")]
        public DateTime CreateTime { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [NotMapped,JsonProperty("account")]
        public string Account { get; set; }
        [JsonIgnore]
        public int Creator { get; set; }
        [JsonProperty("orgId")]
        public int OrgId { get; set; }
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        [JsonProperty("updateTime")]
        public DateTime? UpdateTime{ get; set; }
        [JsonIgnore]
        public int? Updater { get; set; }
        [JsonProperty("updateAccount"), NotMapped]
        public string UpdateAccount { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 行事曆資料表
    /// </summary>
    public class Calendar
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 內文
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 發布日期
        /// </summary>
        [JsonProperty("publishDate")]
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }
        /// <summary>
        /// 是否為大事件
        /// </summary>
        [JsonProperty("isBigEvent")]
        public bool IsBigEvent { get; set; }

        /// <summary>
        /// 代碼
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// 檔案編號
        /// </summary>
        [JsonProperty("fileId")]
        public int? FileId { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        [JsonProperty("createDate")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        [JsonProperty("creator")]
        public int Creator { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [JsonProperty("updateDate")]
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        [JsonProperty("updater")]
        public int Updater { get; set; }
    }
}

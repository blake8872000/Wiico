using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 進度表
    /// </summary>
    public class Syllabus : Base.EntityBase
    {
        [JsonProperty("name")]
        public override string Name { get; set; }
        /// <summary>
        /// 進度說明 
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }
        /// <summary>
        /// 所屬課碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string Course_No { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }
        /// <summary>
        /// 設定進度日期
        /// </summary>
        [JsonProperty("syll_Date")]
        public DateTime Syll_Date { get; set; }
        /// <summary>
        /// 識別某個進度 - 用於同步時避免重複更新
        /// </summary>
        [JsonProperty("syll_Guid")]
        public Guid Syll_Guid { get; set; }
        /// <summary>
        /// 是否要顯示於管理介面 - 封存
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        [JsonProperty("externalRid")]
        public int? ExternalRid { get; set; }
    }
}

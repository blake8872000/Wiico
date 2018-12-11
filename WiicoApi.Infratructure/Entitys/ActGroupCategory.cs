using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 分組主表
    /// </summary>
    public class ActGroupCategory : Base.EntityBase
    {
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }

        /// <summary>
        /// 分組標題
        /// </summary>
        [JsonProperty("title")]
        public override string Name { get; set; }

        /// <summary>
        /// 分組類別代碼
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable{ get; set; }

        /// <summary>
        /// 分組類別說明
        /// </summary>
        [JsonProperty("description")]
        public string Content { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class MessageViewModel
    {   /// <summary>
        /// 活動 Guid
        /// </summary>
        [JsonIgnore]
        public Guid EventId { get; set; }

        /// <summary>
        /// 活動 outerKey
        /// </summary>
        [JsonProperty("eventId")]
        public string strEventId { get; set; }

        /// <summary>
        /// 訊息類型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        [JsonProperty("publish_uUtc")]
        public DateTime? Publish_Utc { get; set; }
    }
}
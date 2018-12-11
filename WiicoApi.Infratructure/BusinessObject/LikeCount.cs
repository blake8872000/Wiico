using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 點讚資訊
    /// </summary>
    public class LikeCount
    {
        /// <summary>
        /// 點讚對象
        /// </summary>
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        /// <summary>
        /// 按讚的清單
        /// </summary>
        [JsonProperty("likeInfo")]
        public List<LikeLog> LikeInfo { get; set; }
        /// <summary>
        /// 按讚的清單
        /// </summary>
        [JsonProperty("likeArray")]
        public List<string> LikeArray { get; set; }
        /// <summary>
        /// 是否已經按過讚
        /// </summary>
        [JsonProperty("isLiked")]
        public bool IsLiked { get; set; }
        /// <summary>
        /// 點讚對象是否為留言 - 可能是模組
        /// </summary>
        [JsonProperty("isMessage")]
        public bool IsMessage { get; set; }
        /// <summary>
        /// 某個模組活動的編碼 - 給前端查詢用
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        /// <summary>
        /// 給前端用的資訊
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

    }
}

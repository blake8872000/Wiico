using Newtonsoft.Json;
using System;
using System.Text;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class ActivitysLatest
    {
        /// <summary>
        /// 活動代碼
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        [JsonProperty("circleId")]
        public int CircleId { get; set; }

        private string _circkeKey;
        [JsonProperty("circleKey")]
        public string CircleKey { get { return _circkeKey?.ToLower(); } set { _circkeKey = value; } }

        [JsonProperty("circleName")]
        public string CircleName { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("memberCount")]
        public int MemberCount { get; set; }

        [JsonIgnore]
        public string MemberName { get; set; }

        [JsonIgnore]
        public string ActType { get; set; }

        [JsonIgnore]
        public string Content { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonIgnore]
        //   public DateTime Created_Utc { get; set; }
        public DateTime Publish_Utc { get; set; }

        [JsonProperty("publish_date")]
        public DateTime Date_Local { get { return Publish_Utc.ToLocalTime(); } }

        [JsonProperty("createTime")]
        public DateTime CreateTime { get { return Publish_Utc.ToLocalTime(); } }

    }
}

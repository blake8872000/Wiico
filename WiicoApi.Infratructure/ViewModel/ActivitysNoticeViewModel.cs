using WiicoApi.Infrastructure.ValueObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class ActivitysNoticeViewModel
    {
        [JsonProperty("unreadCount")]
        public int UnreadCount { get; set; }

        [JsonProperty("data")]
        public IEnumerable<ActivitysNoticeData> Data { get; set; }
    }
}

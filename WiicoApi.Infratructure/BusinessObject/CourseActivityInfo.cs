using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class CourseActivityInfo
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
        [JsonProperty("eventOuterKey")]
        public string EventOuterKey { get; set; }

        /// <summary>
        /// 推播活動對應類型
        /// </summary>
        [JsonProperty("gaEvent")]
        public string GaEvent { get; set; }
    }
}

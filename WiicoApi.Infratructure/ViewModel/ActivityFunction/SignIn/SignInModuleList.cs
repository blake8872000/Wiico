using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.SignIn
{
    public class SignInModuleList 
    {
        /// <summary>
        /// 點名活動的代碼
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }

        /// <summary>
        /// 點名狀態列表
        /// </summary>
        [JsonProperty("signInCount")]
        public List<Infrastructure.ValueObject.SignStatus> SignInCount { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// 活動類型 - 固定為signIn
        /// </summary>
        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }

        [JsonProperty("createTime")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("activityTime")]
        public DateTime ActivityTime { get; set; }

        [JsonProperty("remainSeconds")]
        public double RemainSeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("publish_date")]
        public DateTime Publish_date { get; set; }
        [JsonProperty("myStatus")]
        public int? MyStatus { get; set; }
    }
}

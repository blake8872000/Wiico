using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class DiscussionEvent
    {
        /// <summary>
        /// 活動id
        /// </summary>
        [JsonProperty("outerKey")]
        public Guid OuterKey { get; set; }

        /// <summary>
        /// 建立帳號
        /// </summary>
        [JsonProperty("createAccount")]
        public string CreateAccount { get; set; }

        /// <summary>
        /// 課id
        /// </summary>
        [JsonProperty("classId")]
        public string ClassId { get; set; }

        /// <summary>
        /// 建立者id
        /// </summary>
        [JsonProperty("creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 標籤代碼
        /// </summary>
        [JsonProperty("tagId")]
        public DateTime? TagId { get; set; }
    }
}

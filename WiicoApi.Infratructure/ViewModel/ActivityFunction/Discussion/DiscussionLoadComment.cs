using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    public class DiscussionLoadComment
    {
        /// <summary>
        /// 主題討論的outerKey
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }

        /// <summary>
        /// 新的留言列表
        /// </summary>
        [JsonProperty("comments")]
        public List<DiscussionMessage> Comments { get; set; }
    }
}

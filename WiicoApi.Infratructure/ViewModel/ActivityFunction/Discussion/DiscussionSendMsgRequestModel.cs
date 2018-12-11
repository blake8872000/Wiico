using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    public class DiscussionSendMsgRequestModel
    {
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey{ get; set; }
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("token")]
        public Guid Token { get; set; }
        /// <summary>
        /// 主題討論活動代碼
        /// </summary>
        [JsonProperty("activityOuterKey")]
        public string ActivityOuterKey { get; set; }
        /// <summary>
        /// 要回覆的留言代碼
        /// </summary>
        [JsonProperty("commentOuterKey")]
        public string CommentOuterKey { get; set; }
        /// <summary>
        /// tag的outerKey
        /// </summary>
        [JsonProperty("replyOuterKey")]
        public string ReplyOuterKey { get; set; }
    }
}

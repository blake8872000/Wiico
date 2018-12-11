using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    public class DiscussionUpdateLikeInfo
    {
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
        [JsonProperty("activityOuterKey")]
        public string ActivityOuterKey { get; set; }

        /// <summary>
        /// 留言代碼 - NULL 代表主題討論按讚
        /// </summary>
        [JsonProperty("commentOuterKey")]
        public string CommentOuterKey { get; set; }

        /// <summary>
        /// tag的留言代碼 - NULL 代表沒有tag
        /// </summary>
        [JsonProperty("replyOuterKey")]
        public string ReplyOuterKey { get; set; }

        /// <summary>
        /// 留言類型 - 1 : 主題討論 | 2 : 留言 | 3 : 回覆
        /// </summary>
        [JsonProperty("messageEnum")]
        public int MessageEnum { get; set; }

        /// <summary>
        /// 按讚成員列表
        /// </summary>
        [JsonProperty("likeArray")]
        public string[] LikeArray { get; set; }

        /// <summary>
        /// 是否按讚 - 用於server端方便判斷
        /// </summary>
        [JsonIgnore]
        public bool IsLike { get; set; }
    }
}

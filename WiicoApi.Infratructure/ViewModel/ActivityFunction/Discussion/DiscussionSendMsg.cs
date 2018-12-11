using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    /// <summary>
    /// 主題討論留言後給前端的ViewModel
    /// </summary>
    public class DiscussionSendMsg
    {
        /// <summary>
        /// 主題討論訊息本身編號
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }

        /// <summary>
        /// 主題討論的活動代碼
        /// </summary>
        [JsonProperty("activityOuterKey")]
        public string ActivityOuterKey { get; set; }

        /// <summary>
        /// 留言的代碼
        /// </summary>
        [JsonProperty("commentOuterkey")]
        public string CommentOuterkey { get; set; }

        /// <summary>
        /// 是否為留言 [true:留言 | false:回覆]
        /// </summary>
        [JsonProperty("isComment")]
        public bool IsComment { get; set; }

        /// <summary>
        /// 留言+回覆總數
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 回覆總數
        /// </summary>
        [JsonProperty("commentCount")]
        public int CommentCount { get; set; }

        /// <summary>
        /// 留言詳細資訊
        /// </summary>
        [JsonProperty("comment")]
        public DiscussionMessage Comment { get; set; }
    }
}

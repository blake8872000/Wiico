using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    public class DiscussionCommentDetail
    {
        /// <summary>
        /// 該留言詳細資訊
        /// </summary>
        [JsonProperty("comment")]
        public DiscussionMessage Comment { get; set; }

        /// <summary>
        /// 回覆列表
        /// </summary>
        [JsonProperty("replys")]
        public List<DiscussionMessage> Replys { get; set; }

        /// <summary>
        /// 剩餘舊的留言筆數
        /// </summary>
        [JsonProperty("olderCount")]
        public int OlderCount { get; set; }

        /// <summary>
        /// 留言內總回覆數量
        /// </summary>
        [JsonProperty("commentCount")]
        public int CommentCount { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    /// <summary>
    /// 新增主題討論的Request
    /// </summary>
    public class DiscussionCreateRequestModel
    {

        /// <summary>
        /// 學習圈代碼 - [課程代碼]
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 文字訊息
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 新增訊息者
        /// </summary>
        [JsonProperty("token")]
        public Guid Token { get; set; }

        /// <summary>
        /// 主題活動代碼
        /// </summary>
        [JsonProperty("activityOuterKey")]
        public string ActivityOuterKey { get; set; }

        /// <summary>
        /// 所屬留言 - 可NULL [NULL代表自己是留言]
        /// </summary>
        [JsonProperty("commentOuterKey")]
        public string CommentOuterKey { get; set; }

        /// <summary>
        /// 要回覆的留言代碼 - 可NULL [NULL代表沒有回覆某個留言]
        /// </summary>
        [JsonProperty("replyOuterKey")]
        public string ReplyOuterKey { get; set; }

        /// <summary>
        /// 欲刪除的檔案編號
        /// </summary>
        [JsonProperty("removeFiles")]
        public string[] RemoveFiles { get; set; }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    /// <summary>
    /// 主題討論-留言Model
    /// </summary>
    public class DiscussionMessage : Base.BaseViewModel
    {

        /// <summary>
        /// 留言編號
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// 回覆的留言代碼-[所屬留言可能是NULL]
        /// </summary>
        [JsonProperty("replyOuterKey")]
        public string ReplyOuterKey { get; set; }

        /// <summary>
        /// 回覆對象-[人姓名可能是NULL]
        /// /// </summary>
        [JsonProperty("replyName")]
        public string ReplyName { get; set; }

        /// <summary>
        /// 該留言回覆數量-可能是NULL
        /// </summary>
        [JsonProperty("replyCount")]
        public int? ReplyCount { get; set; }

        /// <summary>
        /// 訊息建立時間
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 留言/回覆者照片
        /// </summary>
        [JsonProperty("creatorPhoto")]
        public string CreatorPhoto { get; set; }

        /// <summary>
        /// 留言/回覆者姓名
        /// </summary>
        [JsonProperty("creatorName")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 留言/回覆者帳號
        /// </summary>
        [JsonProperty("creatorAccount")]
        public string CreatorAccount { get; set; }

        /// <summary>
        /// 留言內容
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 留言照片
        /// </summary>
        [JsonProperty("photos")]
        public List<FileStorageViewModel> Photos { get; set; }

        /// <summary>
        /// 哪個留言底下的
        /// </summary>
        [JsonIgnore]
        public int? Parent { get; set; }

        /// <summary>
        /// 按讚帳號字串陣列
        /// </summary>
        [JsonProperty("likeArray")]
        public string[] LikeArray { get; set; }
    }
}

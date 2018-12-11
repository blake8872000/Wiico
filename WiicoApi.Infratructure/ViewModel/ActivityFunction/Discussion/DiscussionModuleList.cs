using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion
{
    /// <summary>
    /// 主題討論活動列表專用
    /// </summary>
    public class DiscussionModuleList : Base.BaseViewModel
    {
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// 主題討論標題
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 活動類型 - 固定為discussion
        /// </summary>
        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }

        /// <summary>
        /// 發行日期
        /// </summary>
        [JsonProperty("publish_date")]
        public DateTime Publish_date { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime  CreateTime{ get; set; }

        /// <summary>
        /// 檔案數量
        /// </summary>
        [JsonProperty("fileCount")]
        public int FileCount { get; set; }

        /// <summary>
        /// 回覆數量
        /// </summary>
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 按讚Account陣列
        /// </summary>
        [JsonProperty("likeArray")]
        public List<string> LikeArray { get; set; }
    }
}

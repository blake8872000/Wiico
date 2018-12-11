using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 分組列表專用顯示
    /// </summary>
    public class GroupListViewModel
    {
        /// <summary>
        /// 活動代碼 - 加密
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }

        /// <summary>
        /// 活動代碼 - 原碼
        /// </summary>
        [JsonIgnore]
        public Guid EventId { get; set; }

        /// <summary>
        /// 分組主題
        /// </summary>
        [JsonProperty("groupTitle")]
        public string GroupTitle { get; set; }

        /// <summary>
        /// 分組名稱
        /// </summary>
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 分組排序
        /// </summary>
        [JsonProperty("groupSort")]
        public int? GroupSort { get; set; }

        /// <summary>
        /// 未分組成員數量
        /// </summary>
        [JsonProperty("unGroupCount")]
        public int? UnGroupCount { get; set; }

        /// <summary>
        /// 分組總數量
        /// </summary>
        [JsonProperty("groupCount")]
        public int? GroupCount { get; set; }

        /// <summary>
        /// 建立分組日期
        /// </summary>
        [JsonIgnore]
        public DateTime CreateDateUtc { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        [JsonIgnore]
        public DateTime PublishDateUtc { get; set; }

        /// <summary>
        /// 顯示建立分組日期
        /// </summary>
        [JsonProperty("publish_date")]
        public DateTime CreateDate { get; set; }
    }
}

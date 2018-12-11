using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote
{
    /// <summary>
    /// 投票列表顯示結果
    /// </summary>
    public class VoteListResponse
    {
        [JsonIgnore]
        public int VoteId { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        /// <summary>
        /// 活動代碼 - 加密
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey{ get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
       [JsonIgnore]
        public Guid EventId { get; set; }
        /// <summary>
        /// 投票標題
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 投票內容
        /// </summary>
        [JsonProperty("description")]
        public string Content { get; set; }
        /// <summary>
        /// 投票日期
        /// </summary>
        [JsonProperty("publish_date")]
        public DateTime PublishTime { get; set; }
        /// <summary>
        /// 到場人數
        /// </summary>
        [JsonIgnore]
        public int? PresentCount { get; set; }
        /// <summary>
        /// 是否進行中
        /// </summary>
        [JsonProperty("isStart")]
        public bool IsStart { get; set; }
    }
}

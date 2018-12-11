using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 投票項目
    /// </summary>
    public class ActVoteItem
    {
        /// <summary>
        /// 項目編號
        /// </summary>
        [JsonProperty("chooseID")]
        public int Id { get; set; }
        /// <summary>
        /// 選項排序
        /// </summary>
        [JsonProperty("chooseSort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 選項內容
        /// </summary>
        [JsonProperty("chooseContent")]
        public string Content { get; set; }

        /// <summary>
        /// 選項標題
        /// </summary>
        [MaxLength(100)]
        [JsonProperty("chooseName")]
        public string Title { get; set; }

        /// <summary>
        /// 選項參與率
        /// </summary>
        [JsonProperty("chooseRate")]
        public double ChooseRate { get; set; }
        
        /// <summary>
        /// 選項人數
        /// </summary>
        [JsonProperty("chooseCount")]
        public int ChooseCount { get; set; }

        /// <summary>
        /// 所屬投票活動
        /// </summary>
        [JsonIgnore]
        public int ActVoteId { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote
{
    public class VoteViewModel : ValueObject.ActivityBase
    {
        /// <summary>
        /// 是否進行中
        /// </summary>
        [JsonProperty("isStart")]
        public bool IsStart { get; set; }
  
        /// <summary>
        /// 投票選項標題
        /// </summary>
        [JsonProperty("title")]
        public string Title{ get; set; }

        /// <summary>
        ///  投票選項內容
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 在場人數
        /// </summary>
        [JsonProperty("presentCount")]
        public int PresentCount { get; set; }

        /// <summary>
        /// 參與人數
        /// </summary>
        [JsonProperty("participateCount")]
        public int ParticipateCount { get; set; }

        /// <summary>
        /// 參與率
        /// </summary>
        [JsonProperty("participateRate")]
        public double ParticipateRate { get; set; }

        /// <summary>
        /// 投票選項列表
        /// </summary>
        [JsonProperty("voteItems")]
        public List<Entity.ActVoteItem> VoteItems { get; set; }
    }
}

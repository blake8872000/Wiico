using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActGroupMember
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 第二層分組代碼
        /// </summary>
        [JsonProperty("groupId")]
        public int GroupId { get; set; }
        /// <summary>
        /// 人員代碼
        /// </summary>
        [JsonProperty("memberId")]
        public int MemberId { get; set; }
        /// <summary>
        /// 是否為組長
        /// </summary>
        [JsonProperty("isLeader")]
        public bool IsLeader { get; set; }

        /// <summary>
        /// 判斷是否為分組成員
        /// </summary>
        [JsonProperty("isGroupMember")]
        public bool IsGroupMember { get; set; }
    }
}

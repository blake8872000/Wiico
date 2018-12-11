using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class GroupMember
    {
        /// <summary>
        /// 成員編號
        /// </summary>
        [JsonProperty("memberId")]
        public int Id { get; set; }

        /// <summary>
        /// 成員帳號
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }

        /// <summary>
        /// 成員姓名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 成員頭像
        /// </summary>
        [JsonProperty("photo")]
        public string Photo { get; set; }

        /// <summary>
        /// 是否為組長
        /// </summary>
        [JsonProperty("isLeader")]
        public bool IsLeader { get; set; }
    }
}

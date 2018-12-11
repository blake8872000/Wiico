using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberRoleInfo
    {
        /// <summary>
        /// 角色資訊
        /// </summary>
        [JsonProperty("roleInfo")]
        public Infrastructure.Entity.LearningRole RoleInfo { get; set; }
        /// <summary>
        /// 成員列表
        /// </summary>
        [JsonProperty("memberList")]
        public IEnumerable<Infrastructure.Entity.Member> MemberList { get; set; }
    }
}

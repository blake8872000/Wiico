using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class LearningCircleMemberList
    {
        /// <summary>
        /// 角色資訊
        /// </summary>
        [JsonProperty("roleInfo")]
        public Entity.LearningRole RoleInfo { get; set; }

        /// <summary>
        /// 所屬角色成員
        /// </summary>
        [JsonProperty("memberInfo")]
        public List<Entity.Member> MemberInfo { get; set; }
    }
}

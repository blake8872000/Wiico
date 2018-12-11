using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class LearningCircleMembers
    {
        /// <summary>
        /// 角色資訊
        /// </summary>
        public Entity.Member MemberInfo { get; set; }

        /// <summary>
        /// 所屬角色成員
        /// </summary>
        public List<Entity.LearningRole> RoleInfo { get; set; }
    }
}

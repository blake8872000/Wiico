using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 用於放 viewModel MemberRoleList 中的MemberRoleInfo
    /// </summary>
    public class MemberRoleInfo
    {
        /// <summary>
        /// 角色代碼
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 角色名稱
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否能修改
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// 成員代碼
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 該成員是否為組織管理員
        /// </summary>
        public bool IsOrgAdmin { get; set; }
        /// <summary>
        /// 該角色是否為管理員
        /// </summary>
        public bool IsAdminRole { get; set; }

    }
}

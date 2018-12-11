using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class SessionData
    {
        [Display(Name = "使用者代碼")]
        public int LoginMemberId { get; set; }

        [Display(Name = "帳號")]
        public string LoginAccount { get; set; }

        [Display(Name = "模擬登入的帳號")]
        public string SimulateAccount { get; set; }

        [Display(Name = "姓名")]
        public string LoginName { get; set; }

        [Display(Name = "組織")]
        public int OrgId { get; set; }

        [Display(Name = "登入驗證碼")]
        public Guid Token { get; set; }
        [Display(Name = "角色代碼")]
        public int RoleId { get; set; }
        /// <summary>
        /// 是否為系統管理者
        /// </summary>
        [Display(Name = "系統管理員")]
        public bool IsSystemManager { get; set; }
        /// <summary>
        /// 是否為組織管理員
        /// </summary>
        [Display(Name = "組織管理員")]
        public bool IsOrgAdmin { get; set; }
        /// <summary>
        /// 部門角色清單
        /// </summary>
        [Display(Name = "部門角色")]
        public List<Dept> DeptAdminList { get; set; }
        public string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", LoginName, LoginAccount);
            }
        }

        public Dictionary<int, string> RoleNames
        {
            get
            {
                var dict = new Dictionary<int, string>
                {
                    {1, "系統管理員" },
                    {2, "組織管理員" },
                    {3, "專員" }
                };
                return dict;
            }
        }

        public int[] SystemRoles
        {
            get
            {
                if (systemRoles == null)
                {
                    var roles = new List<int>();
                    if (IsSystemManager) roles.Add(1);
                    if (IsOrgAdmin) roles.Add(2);
                    if (DeptAdminList != null) roles.Add(3);
                    systemRoles = roles.ToArray();
                }
                return systemRoles;
            }
        }

        int[] systemRoles = null;

        int? currentSystemRole = null;
        public int CurrentSystemRole
        {
            get
            {
                if (!currentSystemRole.HasValue)
                {
                    currentSystemRole = SystemRoles[0];
                }
                return currentSystemRole.Value;
            }
            set
            {
                currentSystemRole = value;
            }
        }
    }
}

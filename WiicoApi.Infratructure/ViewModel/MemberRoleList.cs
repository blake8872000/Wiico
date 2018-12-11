using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 用於顯示設定角色 - 設定使用者角色
    /// </summary>
    public class MemberRoleList
    {
        /// <summary>
        /// 使用者代碼 - 流水號
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 成員帳號
        /// </summary>
        public string MemberAccount { get; set; }
        /// <summary>
        /// 課程系組
        /// </summary>
        public string CourseSemeClass { get; set; }
        /// <summary>
        /// 角色資訊清單
        /// </summary>
        public List<BusinessObject.MemberRoleInfo> RoleInfo { get; set; }
    }
}

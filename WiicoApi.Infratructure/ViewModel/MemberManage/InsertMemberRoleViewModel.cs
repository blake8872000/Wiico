using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    /// <summary>
    /// 顯示新增學習圈成員列表
    /// </summary>
    public class InsertMemberRoleViewModel
    {
        /// <summary>
        ///角色列表
        /// </summary>
        public List<Infrastructure.Entity.LearningRole> Roles { get; set; }
        /// <summary>
        /// 未加入成員 - 同組織
        /// </summary>
        public List<Infrastructure.Entity.Member> Members { get; set; }
        /// <summary>
        /// 課程代碼
        /// </summary>
        public string CircleKey { get; set; }
    }
}

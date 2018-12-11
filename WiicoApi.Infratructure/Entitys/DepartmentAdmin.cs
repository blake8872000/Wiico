using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{

    /// <summary>
    ///部門角色成員關聯表
    /// </summary>
    public class DepartmentAdmin
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 成員代碼
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 部門代碼
        /// </summary>
        public int DeptId { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Visibility { get; set; }
    }
}

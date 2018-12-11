using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class RoleModuleFunction
    {
        /// <summary>
        /// 角色代碼 - 流水號
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 角色名稱
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 功能權限清單[顯示有 / 無權限]
        /// </summary>
        public List<BusinessObject.ModuleFunctions> FunctionAuth { get; set; }
    }
}

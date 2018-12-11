using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class AuthModuleInfo
    {
        /// <summary>
        /// 模組代碼 - 流水號
        /// </summary>
        public int ModuleId { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模組底下的功能 + 行為群
        /// </summary>
        public ValueObject.ModuleFunctionInfo[] Functions { get; set; }
    }
}

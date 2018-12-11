using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class ModuleFunctions
    {
        /// <summary>
        /// 功能代碼 - 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 功能名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 有 / 無 權限
        /// </summary>
        public bool Enable { get; set; }
    }
}

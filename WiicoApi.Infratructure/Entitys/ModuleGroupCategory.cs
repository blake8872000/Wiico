using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 分組與所有需要分組的事件關聯
    /// </summary>
    public class ModuleGroupCategory 
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 分組主表代碼
        /// </summary>
        public int GCId { get; set; }
        /// <summary>
        /// 事件關聯的Key
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// 是否為學習圈內的活動
        /// </summary>
        public bool IsActivity { get; set; }
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

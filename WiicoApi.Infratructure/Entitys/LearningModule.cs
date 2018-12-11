using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 學習模組資料表
    /// </summary>
    public class LearningModule
    {
        /// <summary>
        /// 模組編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模組代碼
        /// </summary>
        public string ModuleKey { get; set; }
    }
}

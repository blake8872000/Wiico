using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    //模組內的"功能"資料表
    public class ModuleFuntions : Base.EntityBase
    {
        /// <summary>
        /// 模組功能名稱
        /// </summary>
        [MaxLength(50)]
        public override string Name { get; set; }

        /// <summary>
        /// 模組編號 - 流水號
        /// </summary>
        public int ModulesId { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
        
        /// <summary>
        /// 用於註冊iThink時的Key
        /// </summary>
        [MaxLength(100)]
        public string OutSideKey { get; set; }
        /// <summary>
        /// 是否為管理者功能
        /// </summary>
        public bool IsAdminAuth { get; set; }
        /// <summary>
        /// 是否為一般使用者功能
        /// </summary>
        public bool IsNormalAuth { get; set; }
    }
}

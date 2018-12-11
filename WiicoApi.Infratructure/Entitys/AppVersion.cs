using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class AppVersion
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 程式系統 - Android | IOS
        /// </summary>
        public string AppSystem { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateUtcDate { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UpdateUtcDate { get; set; }
        /// <summary>
        /// 版本號
        /// </summary>
        public string Version { get; set; }
    }
}

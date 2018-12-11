using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 推播記錄
    /// </summary>
    public class PushLog
    {
        /// <summary>
        /// 編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 推播內容
        /// </summary>
        public int PushDataId { get; set; }
        /// <summary>
        /// 推播對象
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 推播時間
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 預約推播時間
        /// </summary>
        public DateTime? PublishDate { get; set; }

        public bool Enable { get; set; }
    }
}

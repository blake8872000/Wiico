using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 活動進度關聯資料表
    /// </summary>
    public class ActivitySyllabus
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 活動編號
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// 進度編號
        /// </summary>
        public int SyllabusId { get; set; }

    }
}

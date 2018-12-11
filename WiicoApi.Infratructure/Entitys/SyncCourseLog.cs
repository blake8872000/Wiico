using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 同步課程的基本資訊
    /// </summary>
    public class SyncCourseLog : Base.ChangeTimeBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 課程代碼
        /// </summary>
        public string CourseCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 學生上傳活動作業紀錄
    /// </summary>
    public class StuHomeworkRecord
    {
        /// <summary>
        /// 作業活動ID
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// 作業活動ID
        /// </summary>
        public int EventLogId { get; set; }
        /// <summary>
        /// 上傳檔案數量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 上傳檔案總量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 作業活動及目前作業紀錄
        /// </summary>
        public Infrastructure.ViewModel.HomeWorkViewModel HomeworkRecord { get; set; }
        /// <summary>
        /// 個人上傳活動紀錄
        /// </summary>
        public Infrastructure.ViewModel.HomeWorkViewModel HomeworkRecordByPerson { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActHomeWork : Base.ChangeTimeBase
    {
        /// <summary>
        /// 作業活動id - 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        public int LearningId { get; set; }
        /// <summary>
        /// 作業活動 Guid
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// 活動名稱
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 逾期時間
        /// </summary>
        public DateTime? OverdueDate { get; set; }
        /// <summary>
        /// 作業活動說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 存在於哪個drive資料夾
        /// </summary>
        public string GoogleDriveFolder { get; set; }
        /// <summary>
        /// 老師上傳的講義總數量
        /// </summary>
        public int LectureCount { get; set; }
        /// <summary>
        /// 允許發布成績
        /// </summary>
        public bool AllowRelease { get; set; }
        /// <summary>
        /// 是否發行過
        /// </summary>
        public bool Released { get; set; }
        /// <summary>
        /// 允許遲交
        /// </summary>
        public bool AllowOverDue { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Visibility { get; set; }
    }
}

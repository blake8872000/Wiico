using System;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    public class AttendanceRecord
    {
        /// <summary>
        /// 出缺勤紀錄id - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        public int LearningId { get; set; }

        /// <summary>
        /// 活動 Guid - 與 Activitys 產生關聯
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 學生MemId
        /// </summary>
        public int StudId { get; set; }

        /// <summary>
        /// 出缺勤狀態:1出席,2缺席,3遲到,4早退,5請假
        /// </summary>
        [MaxLength(2)]
        public string Status { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    public class AttendanceLeaveLog
    {
        /// <summary>
        /// 請假申請歷程id - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 請假申請id - 流水號[外來鍵]
        /// </summary>
        public int LeaveId { get; set; }

        /// <summary>
        /// 建立者MemId
        /// </summary>
        public int Creator { get; set; }

        /// <summary>
        /// 原申請單狀態:00 未送出，10 審核中，20 已完成，40 作廢，50 申請抽回，60 駁回
        /// </summary>
        [MaxLength(2)]
        public string OldStatus { get; set; }

        /// <summary>
        /// 新申請單狀態:00 未送出，10 審核中，20 已完成，40 作廢，50 申請抽回，60 駁回
        /// </summary>
        [MaxLength(2)]
        public string NewStatus { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class AttendanceLeaveViewData
    {
        /// <summary>
        /// 請假申請id - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 請假單代碼
        /// </summary>
        [JsonIgnore]
        public Guid eventId { get; set; }

        /// <summary>
        /// 請假單代碼
        /// </summary>
        public string outerKey { get; set; }

        /// <summary>
        /// 申請人姓名
        /// </summary>
        public string StudName { get; set; }

        /// <summary>
        /// 請假日期
        /// </summary>
        public DateTime LeaveDate { get; set; }

        /// <summary>
        /// 申請假別:10 事假，20 病假
        /// </summary>
        public string LeaveType { get; set; }

        /// <summary>
        /// 申請主旨
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 申請說明
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 申請狀態:00 已作廢,10 已完成,20 審核中,30 申請人抽回,40 已駁回
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 申請狀態名稱:00 已作廢,10 已完成,20 審核中,30 申請人抽回,40 已駁回
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// 審核批注
        /// </summary>
        public string Comment { get; set; }
    }
}

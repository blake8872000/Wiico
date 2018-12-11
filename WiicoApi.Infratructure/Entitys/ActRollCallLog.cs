using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActRollCallLog : Base.ChangeTimeBase
    {
        /// <summary>
        /// 點名紀錄編號 - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 點名活動Id - 流水號
        /// </summary>
        public int RollCallId { get; set; }
        
        /// <summary>
        /// 簽到時間
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// 被簽到的學生MemId
        /// </summary>
        public int StudId { get; set; }

        /// <summary>
        /// 被簽到的狀態:1出席,2缺席,3遲到,4早退,5請假
        /// </summary>
        [MaxLength(2)]
        public string Status { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }
    }
}

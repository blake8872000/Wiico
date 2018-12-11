using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 投票活動資料表
    /// </summary>
    public class ActVote
    {
        /// <summary>
        /// 投票活動編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 投票活動代碼
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 投票標題
        /// </summary>
        [MaxLength(100)]
        public string  Title { get; set; }

        /// <summary>
        /// 投票說明
        /// </summary>
        public string  Content { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateDateUtc { get; set; }

        /// <summary>
        /// 建立者編號
        /// </summary>
        public int Creator { get; set; }

        /// <summary>
        /// 是否開始
        /// </summary>
        public bool IsStart { get; set; }

        /// <summary>
        /// 在場人數
        /// </summary>
        public int? PresentCount { get; set; }
    }
}

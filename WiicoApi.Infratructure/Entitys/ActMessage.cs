using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActMessage : Base.ChangeTimeBase
    {
        /// <summary>
        /// 訊息活動id - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        public int LearningId { get; set; }

        /// <summary>
        /// 訊息活動 Guid
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 訊息類型: text
        /// </summary>
        [MaxLength(20)]
        public string Type { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Visibility { get; set; }
    }
}

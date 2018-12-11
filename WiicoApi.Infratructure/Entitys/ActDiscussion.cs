using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActDiscussion : Base.EntityBase
    {
        /// <summary>
        /// 主題討論附件檔案
        /// </summary>
        public string GoogleDriveUrl { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Visibility { get; set; }
        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        public int LearningId { get; set; }
        /// <summary>
        /// 活動 Guid
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// 活動說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 主題討論檔案總數
        /// </summary>
        public int FileCount { get; set; }
        /// <summary>
        /// 標籤代碼
        /// </summary>
        public int? TagId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 推播內容
    /// </summary>
    public class PushData
    {
        /// <summary>
        /// 編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 推播功能代號
        /// </summary>
        public string SystemId { get; set; }
        /// <summary>
        /// 活動功能代號 - 用於分辨活動功能
        /// </summary>
        public string GaEvent { get; set; }
        /// <summary>
        /// 學習圈代號 - 用於進入課程內頁
        /// </summary>
        public string CircleKey { get; set; }
        /// <summary>
        /// 所屬活動代號 - 可null用於進入活動內頁
        /// </summary>
        public string EventOuterKey { get; set; }
    }
}

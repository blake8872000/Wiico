using System;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 訊息活動清單
    /// </summary>
    public class Activitys : Base.ChangeTimeBase
    {
        /// <summary>
        /// 訊息活動編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 訊息活動發送群組編號-課程：SectionId，社團：，分組：
        /// </summary>
        public string ToRoomId { get; set; }

        /// <summary>
        /// 活動類別 - Modules OutSideKey
        /// </summary>
        [MaxLength(100)]
        public string ModuleKey { get; set; }

        /// <summary>
        /// 活動關聯資料Guid - eventId
        /// </summary>
        public Guid OuterKey { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 活動時間
        /// </summary>
        public DateTime? ActivityDate { get; set; }

        /// <summary>
        /// 持續時間(秒)
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// 是否為活動
        /// </summary>
        public bool IsActivity { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        public DateTime? Publish_Utc { get; set; }

        /// <summary>
        /// 卡片已發出
        /// </summary>
        public bool? CardisShow { get; set; }

    }
}

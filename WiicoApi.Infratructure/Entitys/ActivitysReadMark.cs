using System;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 訊息活動已讀紀錄
    /// </summary>
    public class ActivitysReadMark
    {
        /// <summary>
        /// 紀錄編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 訊息活動發送群組編號
        /// </summary>
        public string ToRoomId { get; set; }

        /// <summary>
        /// 對象 member id
        /// </summary>
        public int memberId { get; set; }

        /// <summary>
        /// 上次已讀活動編號 - 最舊的一筆 id
        /// </summary>
        public int LastReadActivityIdBegin { get; set; }

        /// <summary>
        /// 上次已讀活動編號 - 最新的一筆 id
        /// </summary>
        public int LastReadActivityIdEnd { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// 是否顯示
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UpdateDate_Utc { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActivitysNotices
    {
        /// <summary>
        /// 通知編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 通知活動隸屬的學習圈代碼
        /// </summary>
        public string ToRoomId { get; set; }

        /// <summary>
        /// 對象 member id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 通知內文
        /// </summary>
        [MaxLength(150)]
        public string NoticeContent { get; set; }        

        /// <summary>
        /// 此筆通知是否點選讀取
        /// </summary>
        public bool HasClick { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

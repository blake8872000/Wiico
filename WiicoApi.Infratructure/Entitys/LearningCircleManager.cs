using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 學習圈管理者
    /// </summary>
    public class LearningCircleManager
    {
        /// <summary>
        /// 學習圈管理者編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 管理者編號
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 學習圈編號
        /// </summary>
        public int LearningCircleId { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateUtcDate { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        public int Creator { get; set; }
        /// <summary>
        /// 加入方法  0 : 系統設定 | 3 : Email加入
        /// </summary>
        public int ResType { get; set; }
    }
}

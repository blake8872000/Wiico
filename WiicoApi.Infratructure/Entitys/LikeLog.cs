using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 點讚清單
    /// </summary>
    public class LikeLog : Base.ChangeTimeBase
    {
        /// <summary>
        /// 點讚編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 吃模組或模組留言的eventId
        /// </summary>
        public Guid OuterKey { get; set; }

        /// <summary>
        /// 點讚者 memberId
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// [舊]點讚類型 10:讚；20:分享 ; 
        /// [新]點讚類型 10:留言；20:模組活動 ; 
        /// </summary>
        public bool IsMsg { get; set; }
    }
}

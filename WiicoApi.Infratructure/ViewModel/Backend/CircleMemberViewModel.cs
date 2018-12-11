using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 顯示課程成員管理頁面資訊
    /// </summary>
    public class CircleMemberViewModel
    {
        /// <summary>
        /// 學習圈列表
        /// </summary>
        public IEnumerable<LearningCircle> LearningCircleList { get; set; }
        public IEnumerable<Member> MemberList { get; set; }
    }
}

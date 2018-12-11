using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActDiscussionMsg : Base.ChangeTimeBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 留言模組總表流水號
        /// </summary>
        public int ActModuleMsgId { get; set; }
        /// <summary>
        /// 主題討論代碼
        /// </summary>
        public int ActDiscussionId { get; set; }
        /// <summary>
        /// 對外呼叫用 - [like + ActModuleMsg 使用]
        /// </summary>
        public Guid OuterKey { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.DataTransferObject
{
    public class CommonValue
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public Guid token { get; set; }

        /// <summary>
        /// 活動代碼
        /// </summary>
        public string outerKey { get; set; }

        /// <summary>
        /// 學習圈代碼
        /// </summary>
        public string circleKey { get; set; }

        /// <summary>
        /// 分類代碼
        /// </summary>
        public Guid eventId { get; set; }

        /// <summary>
        /// 使用者編號
        /// </summary>
        public int memberId { get; set; }

        public bool isShowMail { get; set; }

        public string email { get; set; }
    }
}

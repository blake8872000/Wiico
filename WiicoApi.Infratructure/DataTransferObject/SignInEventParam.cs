using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.DataTransferObject
{
    public class SignInEventParam
    {
        /// <summary>
        /// 查詢者memberId
        /// </summary>
        public int? MemberId { get; set; }

        /// <summary>
        /// 查詢者是否有權限查看每位成員的簽到記錄
        /// </summary>
        public bool IsAdminRole { get; set; }

        private List<Guid> _eventIds = new List<Guid>();
        /// <summary>
        /// 查詢特定點名活動，指定本參數值
        /// </summary>
        public List<Guid> EventIds { get { return _eventIds; } set { _eventIds = value; } }

        /// <summary>
        /// 查詢學習圈內所有點名活動，指定本參數值(可搭配分頁)
        /// </summary>
        public string CircleKey { get; set; }

        /// <summary>
        /// 不包含已刪除的活動
        /// </summary>
        public bool NotDeleted { get; set; }

        /// <summary>
        /// 分頁-每頁筆數
        /// </summary>
        public int? Rows { get; set; }

        /// <summary>
        /// 分頁-第幾頁
        /// </summary>
        public int? Pages { get; set; }
    }
}

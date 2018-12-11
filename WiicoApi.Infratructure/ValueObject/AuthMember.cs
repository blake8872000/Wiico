using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class AuthMember
    {
        /// <summary>
        /// 使用者代碼 - 流水號
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// 使用者名稱 - 姓名
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 使用者帳號 - 登入帳號
        /// </summary>
        public string Account { get; set; }

        public string Picture { get; set; }
    }
}

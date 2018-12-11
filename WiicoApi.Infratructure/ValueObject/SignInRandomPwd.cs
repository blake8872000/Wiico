using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public enum RandomPwd
    {
        /// <summary>
        /// 數字型別
        /// </summary>
        Number = 1,

        /// <summary>
        /// 字母大寫
        /// </summary>
        Uppercase = 2,

        /// <summary>
        /// 字母小寫
        /// </summary>
        Lowercase = 3,
    }
}

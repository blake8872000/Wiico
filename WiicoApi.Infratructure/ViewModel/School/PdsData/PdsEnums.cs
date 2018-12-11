using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    /// <summary>
    /// 通過狀態
    /// </summary>
    public enum PdsEnums : int
    {
        /// <summary>
        /// 未完成
        /// </summary>
        UnComplete = -1,
        /// <summary>
        /// 編輯中
        /// </summary>
        Editing = 0,
        /// <summary>
        /// 一審通過
        /// </summary>
        PassStep1 = 1,
        /// <summary>
        /// 二審通過
        /// </summary>
        PassStep2 = 2
    }
}


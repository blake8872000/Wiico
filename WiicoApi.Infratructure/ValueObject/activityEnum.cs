using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 要求資料類型
    /// </summary>
    public enum activityEnum : int
    {
        /// <summary>
        /// 上傳紀錄
        /// </summary>
        Upload = 0,
        /// <summary>
        /// 檔案列表
        /// </summary>
        Material = 1,
        /// <summary>
        /// 簽到退紀錄
        /// </summary>
        SignIn = 2,
        /// <summary>
        /// 主題討論列表
        /// </summary>
        Discussion = 3,
        /// <summary>
        /// 分組活動列表
        /// </summary>
        Group = 4,
        /// <summary>
        /// 請假紀錄
        /// </summary>
        Leave = 5,
               /// <summary>
               /// 投票紀錄
               /// </summary>
        Vote = 6
    }
}

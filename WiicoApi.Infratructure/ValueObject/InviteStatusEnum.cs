using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
     public enum InviteStatusEnum : int
    {
        /// <summary>
        /// 成功
        /// </summary>
        success = 0,
        /// <summary>
        /// 邀請碼失效
        /// </summary>
        inviteError = 1,
        /// <summary>
        /// 結束課程邀請
        /// </summary>
        EndInvite = 2,
        /// <summary>
        /// 帳號無法使用在此組織
        /// </summary>
        AccountNotAllow = 3
    }
}

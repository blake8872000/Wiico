using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public enum enumLeaveType :int
    {
        /// <summary>
        /// 病假
        /// </summary>
        SickLeave = 1,
        /// <summary>
        /// 事假
        /// </summary>
        PersonalLeave = 2,
        /// <summary>
        /// 公假
        /// </summary>
        LeaveForStatutory = 3,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 4
    }
}

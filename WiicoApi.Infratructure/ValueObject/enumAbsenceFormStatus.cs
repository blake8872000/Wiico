using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public enum enumAbsenceFormStatus : int
    {
        /// <summary>
        /// 已作廢
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 已完成
        /// </summary>
        Pass = 1,
        /// <summary>
        /// 待審核
        /// </summary>
        Wait = 2,
        /// <summary>
        /// 已抽回
        /// </summary>
        Recall = 3,
        /// <summary>
        /// 已駁回
        /// </summary>
        Reject = 4
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 在學狀態
    /// </summary>
    public enum GraduationStatus : int
    {
        residence = 10, //在學
        graduation = 20, //畢業
        off = 30, //休學
        suspend = 40 //退學
    }
}

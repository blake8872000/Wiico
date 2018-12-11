using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public enum AttendanceStateEnum : int
    {
        UnKnow = 0,
        未開放您參加此活動 = -1,
        出席 = 1,
        缺席 = 2,
        遲到 = 3,
        早退= 4,
        請假=5
    }
}

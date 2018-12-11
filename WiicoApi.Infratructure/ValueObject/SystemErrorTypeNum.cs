using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public enum SystemErrorTypeNum : int
    {
        SystemError = 1,
        SignalRError = 2,
        UIError = 3
    }
}

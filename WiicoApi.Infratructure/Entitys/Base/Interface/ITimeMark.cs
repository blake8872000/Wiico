using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity.Base.Interface
{
    public interface ITimeMark
    {
        Property.TimeData Created { get; set; }
        Property.TimeData Updated { get; set; }
        Property.TimeData Deleted { get; set; }
    }

}

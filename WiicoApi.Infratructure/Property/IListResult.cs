using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Property
{
    public interface IListResult<T>
    {
        T[] Data { get; set; }
    }
}

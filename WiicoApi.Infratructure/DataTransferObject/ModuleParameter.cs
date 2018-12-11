using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.DataTransferObject
{
    public class ModuleParameter
    {
        public string ModuleKey { get; set; }
        public string CircleKey { get; set; }
        public int MemberId { get; set; }
        public Guid EventId { get; set; }
        public int? Pages { get; set; }
        public int? Rows { get; set; }
    }
}

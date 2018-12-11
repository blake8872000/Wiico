using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class ModuleFunctionInfo
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public bool Enable { get; set; }
        public List<AuthMember> Members { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class LoginTokenInfo
    {
        public int MemberId { get; set; }
        public int OrgId { get; set; }
        public bool IsOrgAdmin { get; set; }
    }
}

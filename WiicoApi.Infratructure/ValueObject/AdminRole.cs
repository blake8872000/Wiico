using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class AdminRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsAdminRole { get; set; }
    }
}

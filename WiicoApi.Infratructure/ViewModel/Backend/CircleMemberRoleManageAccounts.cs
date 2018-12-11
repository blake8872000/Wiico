using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberRoleManageAccounts
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
    }
}

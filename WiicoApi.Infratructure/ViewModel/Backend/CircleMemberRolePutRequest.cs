using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberRolePutRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
        [JsonProperty("accounts")]
        public List<string>Accounts { get; set; }
    }
}

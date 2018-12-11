using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class PersonPutRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("orgId")]
        public int OrgId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("roleId")]
        public int? RoleId { get; set; }
        [JsonProperty("photo")]
        public string Photo { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("isShowEmail")]
        public bool IsShowEmail { get; set; }
    }
}

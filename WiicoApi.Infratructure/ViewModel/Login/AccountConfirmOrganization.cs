using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class AccountConfirmOrganization
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("orgName")]
        public string OrgName { get; set; }
        [JsonProperty("orgCode")]
        public string OrgCode{ get; set; }
    }
}

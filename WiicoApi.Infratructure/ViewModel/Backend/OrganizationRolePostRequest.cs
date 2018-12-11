using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class OrganizationRolePostRequest : OrganizationRoleGetRequest
    {
        [JsonProperty("orgRoles")]
        public List<OrganizationRole> OrgRoles { get; set; }
    }
}

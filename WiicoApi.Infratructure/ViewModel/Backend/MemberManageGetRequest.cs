using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class MemberManageGetRequest : Base.BackendBaseRequest
    {
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }
    }
}

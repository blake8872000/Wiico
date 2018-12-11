using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class MemberManageDeleteRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("members")]
        public List<int> Members { get; set; }
    }
}

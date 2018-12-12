using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class UserGetResponse
    {
        [JsonProperty("pages")]
        public int? Pages { get; set; }
        [JsonProperty("nextPage")]
        public int?  NextPage{ get; set; }
        [JsonProperty("backPage")]
        public int? BackPage { get; set; }
        [JsonProperty("users")]
        public List<UserGetResponseData> Users { get; set; }
    }
}

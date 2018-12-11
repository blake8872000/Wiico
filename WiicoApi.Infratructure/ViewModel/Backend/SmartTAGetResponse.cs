using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class SmartTAGetResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("datas")]
        public List<SmartTAGetResponseData> Datas { get; set; }
    }
}

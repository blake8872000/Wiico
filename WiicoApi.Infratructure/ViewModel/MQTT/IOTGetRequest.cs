using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTGetRequest
    {
        [JsonProperty("iCanToken")]
        public string ICanToken { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("classId")]
        public string ClassId { get; set; }
    }
}

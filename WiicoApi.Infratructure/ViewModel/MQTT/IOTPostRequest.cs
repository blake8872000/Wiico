using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTPostRequest
    {
        [JsonProperty("iCanToken")]
       public string ICanToken { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTSensorResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("value")]
        public string[] Value { get; set; }
    }
}

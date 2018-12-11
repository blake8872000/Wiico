using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTProjectAvgDataResponse
    {
        [JsonProperty("statusID")]
        public int StatusID { get; set; }
        [JsonProperty("statusCountAvg")]
        public double StatusCountAvg { get; set; }
        [JsonProperty("percentage")]
        public double Percentage { get; set; }
    }
}

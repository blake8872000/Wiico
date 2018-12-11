using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTProjectAverageData
    {
        [JsonProperty("chooseID")]
       public int ChooseID { get; set; }
        [JsonProperty("chooseCountAvg")]
        public double ChooseCountAvg { get; set; }
        [JsonProperty("percentage")]
        public double Percentage { get; set; }
    }
}

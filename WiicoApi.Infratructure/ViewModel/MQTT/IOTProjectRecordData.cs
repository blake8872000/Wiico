using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTProjectRecordData
    {
        [JsonProperty("chooseID")]
        public int ChooseID { get; set; }
        [JsonProperty("chooseCount")]
        public int ChooseCount { get; set; }
    }
}

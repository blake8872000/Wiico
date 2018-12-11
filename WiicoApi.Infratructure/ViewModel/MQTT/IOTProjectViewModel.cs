using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class IOTProjectViewModel<T>
    {
        [JsonProperty("presentCount")]
        public decimal PresentCount { get; set; }
        [JsonProperty("data")]
        public List<T> Data { get; set; }
        [JsonProperty("recordTime")]
        public DateTime RecordTime { get; set; }

    }
}

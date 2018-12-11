using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IOTProjectMoctionViewModel<T> 
    {
        /// <summary>
        /// 參與人數
        /// </summary>
        [JsonProperty("participateCount")]
        public int ParticipateCount { get; set; }
        /// <summary>
        /// 參與率
        /// </summary>
        [JsonProperty("participateRate")]
        public double ParticipateRate { get; set; }
        [JsonProperty("presentCount")]
        public decimal PresentCount { get; set; }
        [JsonProperty("status")]
        public List<T> Status { get; set; }
        [JsonProperty("recordTime")]
        public DateTime RecordTime { get; set; }

    }
}

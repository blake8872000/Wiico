using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// post參數
    /// </summary>
    public class SmartTAPostRequest
    {
        [JsonProperty("circleKeys")]
        public List<string> CircleKeys { get; set; }
        [JsonProperty("classRoomId")]
        public string ClassRoomId { get; set; }
    }
}

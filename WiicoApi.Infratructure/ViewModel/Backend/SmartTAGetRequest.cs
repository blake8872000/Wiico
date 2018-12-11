using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class SmartTAGetRequest
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("classRoomId")]
        public string ClassRoomId { get; set; }
    }
}

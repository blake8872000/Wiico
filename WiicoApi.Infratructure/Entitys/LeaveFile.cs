using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class LeaveFile
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("leaveId")]
        public int LeaveId { get; set; }
        [JsonProperty("googleId")]
        public int GoogleId { get; set; }
    }
}

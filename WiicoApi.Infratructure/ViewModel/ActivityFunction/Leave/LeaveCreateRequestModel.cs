using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class LeaveCreateRequestModel
    {
        [JsonProperty("token")]
        public Guid Token { get; set; }
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("leavedate")]
        public DateTime Leavedate { get; set; }
        [JsonProperty("leavecategoryid")]
        public int Leavecategoryid { get; set; }
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }

    }
}

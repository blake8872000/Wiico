using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School.FeedBack
{
    public class FeedBackPutRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("reContent")]
        public string ReContent { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
    }
}

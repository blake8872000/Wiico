using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School.FeedBack
{
   public class FeedBackPostRequest
    {
        [JsonProperty("feedBackType")]
        public string FeedBackType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("system")]
        public string System { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

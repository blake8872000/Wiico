using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class FIrebasePushPostRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("accounts")]
        public List<string> Accounts { get; set; }
        [JsonProperty("deviceIds")]
        public List <string>DeviceIds { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("systemId")]
        public string SystemId { get; set; }
        [JsonProperty("gaEvent")]
        public string GaEvent { get; set; }
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        [JsonProperty("eventOuterkey")]
        public string EventOuterkey { get; set; }
        [JsonProperty("publishDate")]
        public DateTime? PublishDate { get; set; }
    }
}

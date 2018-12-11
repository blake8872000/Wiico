using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
    public class ResultBaseModel<T>
    {
        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public T[] Data { get; set; }
        [JsonProperty("state")]
        public LogState State { get; set; }
    }
}

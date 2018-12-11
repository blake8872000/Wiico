using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
    /// <summary>
    /// 用於回傳單筆物件資訊
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T>
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
        public T Data { get; set; }
        [JsonProperty("state")]
        public LogState State { get; set; }
    }
}

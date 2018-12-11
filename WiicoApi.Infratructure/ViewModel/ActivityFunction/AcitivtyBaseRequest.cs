using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction
{
    public class AcitivtyBaseRequest
    {
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
  
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("token")]
        public Guid Token { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}

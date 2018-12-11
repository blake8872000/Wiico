using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class UserTokenRequest
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }
        /// <summary>
        /// 裝置存取API代碼
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// 使用者系統存取權杖(for iCan service)
        /// </summary>
        [JsonProperty("iCanToken")]
        public string ICanToken { get; set; }

        /// <summary>
        /// 使用者系統存取權杖(for iCan service)
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

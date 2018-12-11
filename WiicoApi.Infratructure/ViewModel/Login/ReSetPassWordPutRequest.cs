using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class ReSetPassWordPutRequest
    {/// <summary>
    /// 登入者代碼
    /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// 舊的密碼
        /// </summary>
        [JsonProperty("oldPassword")]
        public string OldPassword { get; set; }
        /// <summary>
        /// 登入者所修改的密碼
        /// </summary>
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}

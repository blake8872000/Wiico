using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class ReSetPassWordPostRequest
    {
        /// <summary>
        /// 要寄密碼的mail
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class LoginRequest
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }
        /// <summary>
        /// APP產生裝置亂數代碼
        /// </summary>
        [JsonProperty("phoneID")]
        public string PhoneID { get; set; }
        /// <summary>
        /// 使用者密碼(3DES加密後)
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
        /// <summary>
        /// 手機系統資訊(系統+’;’+版本)
        /// </summary>
        [JsonProperty("requestSystem")]
        public string RequestSystem { get; set; }
        /// <summary>
        /// 推播帳號
        /// </summary>
        [JsonProperty("pushToken")]
        public string PushToken { get; set; }

        /// <summary>
        /// 是否有選取組織
        /// </summary>
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }
    }
}

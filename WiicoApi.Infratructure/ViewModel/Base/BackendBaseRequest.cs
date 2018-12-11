using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
    public class BackendBaseRequest
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
    //    [JsonProperty("AcpdId")]
        public string AcpdId { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }

        /// <summary>
        /// 裝置代碼
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        [JsonProperty("iCanToken")]
        public string ICanToken { get; set; }

        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string  CircleKey { get; set; }

        /// <summary>
        /// 組織代碼
        /// </summary>
        [JsonProperty("orgCode")]
        public string OrgCode { get; set; }

        /// <summary>
        /// 頁碼
        /// </summary>
        public int? Pages { get; set; }
        /// <summary>
        /// 筆數
        /// </summary>
        public int? Rows { get; set; }

    }
}

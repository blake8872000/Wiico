using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 
    /// </summary>
    public class SignInBase : ActivityBase
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("activityId")]
        public int ActivityId { get; set; }

        /// <summary>
        /// 點名活動名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }


        /// <summary>
        /// 手動簽到驗證碼
        /// </summary>
        [JsonProperty("password")]
        public string SignInPwd { get; set; }


        /// <summary>
        /// beacon簽到驗證碼
        /// </summary>
        [JsonProperty("signInKey")]
        public string SignInKey { get; set; }

        /// <summary>
        /// 是不是最新一筆點名(最新一筆才可以編輯跟開始點名)
        /// </summary>
        [JsonProperty("isNewest")]
        public bool IsNewest { get; set; }
    }
}

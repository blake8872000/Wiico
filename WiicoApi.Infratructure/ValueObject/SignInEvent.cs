using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 給APP的點名資料物件
    /// </summary>
    public class SignInEvent : SignInBase
    {
        /// <summary>
        /// 是不是不參與活動(取得的點名是個人的才會使用，Logs.first().Status==AttendanceState.NoNeed )
        /// </summary>
        [JsonProperty("isNoAuth")]
        public bool? IsNoAuth { get; set; }

        /// <summary>
        /// 簽到狀態
        /// </summary>
        [JsonProperty("logs")]
        public SignInLog[] Logs { get; set; }

        /// <summary>
        /// 簽到狀態統計 [出席狀態(Name 中文), 人數]
        /// </summary>
        [JsonProperty("signInCount")]
        public List<SignStatus> SignInCount = new List<SignStatus>();
    }
}

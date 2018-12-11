using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class SetAbsenceFormStatusResponse
    {
        /// <summary>
        /// 點名活動的時間
        /// </summary>
        [JsonProperty("signInDateTime")]
        public DateTime SignInDateTime { get; set; }
        /// <summary>
        /// 點名活動的索引
        /// </summary>
        [JsonProperty("signInOuterKey")]
        public string SignInOuterKey { get; set; }
    }
}

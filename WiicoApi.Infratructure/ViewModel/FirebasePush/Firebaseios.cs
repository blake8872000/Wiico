using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class Firebaseios
    {
        /// <summary>
        /// 帶參數 - authorization :權限 | apns-id:推播編號 | apns-expiration:延遲時間 | apns-priority:優先權(10 | 5) | apns-topic:發送群組 | apns-collapse-id:發送一個推播到多個 same collapse identifier userId
        /// 參見文件 https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CommunicatingwithAPNs.html
        /// </summary>
        public Dictionary<string,string> headers { get; set; }
        /// <summary>
        /// 參見文件 https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html
        /// </summary>
        public JObject payload { get; set; }
    }
}

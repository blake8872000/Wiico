using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class FirebaseAndroid
    {
        /// <summary>
        /// 延遲秒數後送出
        /// </summary>
        public string ttl { get; set; }
        /// <summary>
        /// 訊息的優先權 - normal | high
        /// </summary>
        public string priority { get; set; }
        /// <summary>
        /// 直接推播內容
        /// </summary>
        public FirebaseNotification notification { get; set; }
        /// <summary>
        /// 客製資料結構 -Key Value結構
        /// </summary>
        public JObject data { get; set; }

        /// <summary>
        ///  最多四個字
        /// </summary>
        public string collapseKey { get; set; }
        /// <summary>
        /// Package name of the application where the registration tokens 
        /// </summary>
        public string restrictedPackageName { get; set; }
    }
}

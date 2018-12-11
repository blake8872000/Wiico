using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    /// <summary>
    /// 推播設定
    /// </summary>
    public class SiteConfig
    {
        /// <summary>
        /// 時間格式
        /// </summary>
        public static string DateTimeFormat { get { return "yyyy-MM-ddTHH:mm:ss"; } }

        static Dictionary<string, string> TempConfig = new Dictionary<string, string>();

        static string GetConfig(string key)
        {
            if (!TempConfig.ContainsKey(key))
            {
                TempConfig.Add(key, ConfigurationManager.AppSettings[key]);
            }
            return TempConfig[key];
        }
        /// <summary>
        /// 舊iCan5點名網址
        /// </summary>
        public static string SigninCheckUrl
        {
            get { return GetConfig("ican-signin-url"); }
        }
        /// <summary>
        /// 舊iCan5點名token
        /// </summary>
        public static string SigninCheckToken
        {
            get { return GetConfig("ican-signin-token"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PhotoTempLocation
        {
            get { return GetConfig("photo-temp-location"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string RoomisApiUrl
        {
            get { return GetConfig("roomis-api-url"); }
        }

        //public static string PushToken {
        //	get { return GetConfig("push-token"); }
        //}
        /// <summary>
        /// 
        /// </summary>
        public static string PushSenderId
        {
            get { return GetConfig("push-senderid"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string PushServiceTokenUrl
        {
            get { return GetConfig("push-service-token-url"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PushServiceTokenAccount
        {
            get { return GetConfig("push-service-token-account"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PushServiceTokenPassword
        {
            get { return GetConfig("push-service-token-password"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PushServiceUrl
        {
            get { return GetConfig("push-service-url"); }
        }

        /// <summary>
        /// 提供與App Server交換資料的編解碼作業
        /// </summary>
        public static string DESKey
        {
            get { return GetConfig("des-key"); }
        }
        /// <summary>
        /// 提供與App Server交換資料的編解碼作業
        /// </summary>
        public static string DESIV
        {
            get { return GetConfig("des-iv"); }
        }
    }
}

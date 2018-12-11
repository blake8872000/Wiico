using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Service.Utility
{
    public class OuterKeyHelper
    {
        public static string Base64UrlEncode(string input)
        {
            return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
        }
        public static string Base64UrlDecode<T>(string input)
        {
            return Encoding.UTF8.GetString(Base64UrlDecode(input));
        }
        public static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        // from JWT spec
        public static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
        /// <summary>
        /// 確認outerKey是否為Guid格式還是編譯過的outerKey
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns>活動代碼 或 留言代碼</returns>
        public static Guid? CheckOuterKey(string outerKey)  {
            //為了可能是直接從留言內頁查詢資訊
            var _guid = Guid.NewGuid();
            var checkOuterKey = Guid.TryParse(outerKey, out _guid);

            var eventId = _guid;
            //如果接到為Guid格式，則直接傳msg的function
            try {
                if (checkOuterKey)
                    eventId = _guid;
                else
                    eventId =PageTokenToGuid(outerKey);

                return eventId;
            } catch (Exception ex) {
                return null;
            }
        }
        /// <summary>
        /// 將 guid 轉成 pageToken (base64UrlEncoded)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GuidToPageToken(Guid guid)
        {
            if (guid.Equals(Guid.Empty)) return null;
                return Base64UrlEncode(guid.ToByteArray());
            /*
			return Convert.ToBase64String(guid.ToByteArray())
			.Substring(0, 22)
			.Replace("/", "_")
			.Replace("+", "-");
			*/
        }

        /// <summary>
        /// 將 pageToken 字串轉回 guid
        /// </summary>
        /// <param name="pageTokenString"></param>
        /// <returns></returns>
        public static Guid PageTokenToGuid(string pageTokenString)
        {
            if (string.IsNullOrEmpty(pageTokenString) || pageTokenString.Length != 22) return Guid.Empty;
            try
            {
                return new Guid(Base64UrlDecode(pageTokenString));
                //return new Guid(Convert.FromBase64String(pageTokenString.Replace("_", "/").Replace("-", "+") + "=="));
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }
}

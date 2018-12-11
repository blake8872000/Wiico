using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Service.Utility
{
    public class Encryption
    {

        /// <summary>
        /// 加密SHA256
        /// </summary>
        /// <param name="RealPwd"></param>
        /// <returns></returns>
        public byte[] StringToSHA256(string RealPwd)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            //將字串轉成Byte
            byte[] source = Encoding.Default.GetBytes(RealPwd);
            //開始加密
            byte[] ShaPwd = sha256.ComputeHash(source);
            //解密
            //string result = Convert.ToBase64String(ShaPwd);
            return ShaPwd;
        }

        /// <summary>
        /// 解密字串 - APP登入驗證用
        /// </summary>
        /// <param name="strEncryptPassword">欲解密字串</param>
        /// <param name="key">3DES的key</param>
        /// <returns></returns>
        public string DecryptString(string strEncryptPassword, string key)
        {
            string retValue = string.Empty;
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(strEncryptPassword);

                keyArray = Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("27005858");

                ICryptoTransform cTransform = tdes.CreateDecryptor();

                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                //    Utility.SendMail("icanBridge", string.Format("{0}", ex.ToString()));
                //    LoggerHelper.logger.Error("DecryptString error,data:" + strEncryptPassword);
            }
            return retValue;
        }

        /// <summary>
        /// 加密 - APP登入驗證用
        /// </summary> 
        /// <param name="toEncrypt"></param>
        /// <param name="key">3DES的key</param>
        /// <returns></returns>
        public string EncryptString(string toEncrypt, string key)
        {
            string retValue = string.Empty;

            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

                keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("27005858");

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                retValue = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch
            {
            }

            return retValue;
        }
    }
}

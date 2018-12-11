using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using WiicoApi.Infrastructure.BusinessObject;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.Entity;

namespace WiicoApi.Filter
{
    /// <summary>
    /// 協助判斷請求參數是否正確
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 從請求中取回攔截器置入的 UserToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static UserToken GetUserToken(HttpRequestMessage request)
        {
            object myObject = null;

            if (request.Properties.TryGetValue("UserToken", out myObject))
            {
                return (UserToken)myObject;
            }

            return null;
        }

        /// <summary>
        /// 從請求中取回攔截器置入的 Member
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Member GetMember(HttpRequestMessage request)
        {
            object myObject = null;

            if (request.Properties.TryGetValue("Member", out myObject))
            {
                return (Member)myObject;
            }

            return null;
        }

        /// <summary>
        /// 從請求中取回攔截器置入的 CircleInfo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CircleCacheData GetCircleInfo(HttpRequestMessage request)
        {
            object myObject = null;

            if (request.Properties.TryGetValue("CircleInfo", out myObject))
            {
                return (CircleCacheData)myObject;
            }

            return null;
        }

        /// <summary>
        /// 依照不同請求方式從 actionContext 取回指定的資料
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueByKey(HttpActionContext actionContext, string key)
        {
            string value = null;
            object obj = value;
            /*   if (!actionContext.ActionArguments.TryGetValue(key, out obj))
                   return null;*/
            //從request body 或 queryString中取值
            if (actionContext.ActionArguments.Count != 0)
            {
                var requestService = new Service.Utility.RequestDataHelper<KeyValuePair<string, object>>();
                foreach (var columnKey in actionContext.ActionArguments)
                {
                    //  if (columnKey.Key.ToString().ToLower() == key.ToLower()) {
                    var checkKey = requestService.CheckColumnEmpty(columnKey, key.ToLower());
                    if (checkKey != null)
                        value = checkKey;
                    //  }
                }
                if (value == null)
                    return null;
            }
            else //取得FormData
            {
                var request = actionContext.Request.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(request))
                {
                    if (!actionContext.Request.Content.IsMimeMultipartContent())
                    {
                        NameValueCollection data = HttpUtility.ParseQueryString(request);
                        value = data[key];
                    }
                    else
                    {
                        var multipart = actionContext.Request.Content.ReadAsMultipartAsync().Result;
                        foreach (var httpContent in multipart.Contents)
                        {
                            var name = httpContent.Headers.ContentDisposition.Name.Split('"')[1];
                            if (string.Equals(name, key))
                            {
                                value = httpContent.ReadAsStringAsync().Result;
                                break;
                            }
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 封裝檔案
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<List<RequestFile>> GetRequestFilesAsync(HttpRequestMessage request)
        {
            List<RequestFile> requestFiles = new List<RequestFile>();

            if (request.Content.IsMimeMultipartContent())
            {
                var files = await request.Content.ReadAsMultipartAsync();
                // 取得實際檔案內容
                foreach (var httpContent in files.Contents)
                {
                    if (httpContent.Headers.ContentDisposition.FileName != null)
                    {
                        RequestFile requestFile = new RequestFile();
                        var fileName = httpContent.Headers.ContentDisposition.FileName.ToString().Replace("\"", "");
                        var contentType = httpContent.Headers.ContentType.MediaType;
                        var size = httpContent.Headers.ContentLength;
                        var stream = httpContent.ReadAsStreamAsync().Result;
                        var bytesInStream = httpContent.ReadAsByteArrayAsync().Result;
                        requestFiles.Add(requestFile);
                    }
                }
            }

            return requestFiles;
        }

    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 上傳狀態統計
    /// </summary>
    public class UploadCount
    {
        /// <summary>
        /// 上傳狀態名稱(中文) ex:出席
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 上傳狀態編號(int) ex:1
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// 上傳狀態筆數
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}

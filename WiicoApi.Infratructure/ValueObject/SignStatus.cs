using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 簽到狀態統計
    /// </summary>
    public class SignStatus
    {
        /// <summary>
        /// 簽到狀態名稱(中文) ex:出席
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 簽到狀態編號(int) ex:1
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// 簽到狀態筆數
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}

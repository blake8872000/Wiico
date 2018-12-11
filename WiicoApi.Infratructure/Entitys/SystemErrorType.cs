using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 錯誤類型資料表
    /// </summary>
    public class SystemErrorType
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 錯誤類型名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 錯誤類型顯示名稱
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
    }
}

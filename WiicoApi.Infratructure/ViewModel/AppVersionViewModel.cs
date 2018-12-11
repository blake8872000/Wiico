using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
   public class AppVersionViewModel
    {
        /// <summary>
        /// 名稱
        /// </summary>
       // [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 版本號碼
        /// </summary>
      //  [JsonProperty("version")]
        public string Version { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
      //  [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 檔案大小
        /// </summary>
   //     [JsonProperty("length")]
        public long Length { get; set; }
    }
}

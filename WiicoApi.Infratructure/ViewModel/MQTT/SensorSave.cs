
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class SensorSave
    {
        /// <summary>
        /// 裝置控制代碼
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// 發送日期 YYYY-MM-DDTHH:mm:SS LocalDateTime
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        /// 儲存
        /// </summary>
        [JsonProperty("save")]
        public bool Save { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [JsonProperty("value")]
        public string[] Value { get; set; }

    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    /// <summary>
    /// 裝置遙控設定
    /// </summary>
    public class SensorSetValues
    {
        /// <summary>
        /// 裝置遙控設定值名稱
        /// </summary>
        [JsonProperty("valueName")]
        public string ValueName { get; set; }
        /// <summary>
        /// 裝置遙控設定值
        /// </summary>
        [JsonProperty("value")]
        public string  Value { get; set; }
    }
}

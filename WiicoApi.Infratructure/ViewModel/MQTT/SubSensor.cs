using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class SubSensor
    {
        /// <summary>
        ///
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }
        /// <summary>
        ///裝置遙控類型
        /// </summary>
        [JsonProperty("type")]
        public int Type{ get; set; }
        /// <summary>
        ///裝置編號
        /// </summary>
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        /// <summary>
        ///裝置遙控代碼
        /// </summary>
        [JsonProperty("sensorId")]
        public string SensorId { get; set; }
        /// <summary>
        ///裝置遙控名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        ///裝置遙控單位
        /// </summary>
        [JsonProperty("unit")]
        public string Unit { get; set; }
        /// <summary>
        ///裝置遙控目前設定值
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        /// <summary>
        ///裝置遙控可設定值
        /// </summary>
        [JsonProperty("setValues")]
        public List<SensorSetValues> SetValues { get; set; }
    }
}

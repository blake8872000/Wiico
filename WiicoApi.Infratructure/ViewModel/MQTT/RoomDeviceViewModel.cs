using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    /// <summary>
    /// 教室設備資訊
    /// </summary>
    public class RoomDeviceViewModel
    {
        /// <summary>
        /// 教室名稱
        /// </summary>
        [JsonProperty("roomName")]
        public string  RoomName { get; set; }
        /// <summary>
        /// 溫度
        /// </summary>
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
        /// <summary>
        /// 二氧化碳值
        /// </summary>
        [JsonProperty("co2")]
        public double Co2 { get; set; }
        /// <summary>
        /// 濕度
        /// </summary>
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
        /// <summary>
        /// 設備列表
        /// </summary>
        [JsonProperty("sensorGroups")]
        public List<DeviceViewModel> SensorGroups { get; set; }
    }
}

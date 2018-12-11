using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public class DeviceViewModel
    {
        /// <summary>
        /// 設備編號
        /// </summary>
        [JsonProperty("groupID")]
        public int GroupID { get; set; }
        /// <summary>
        /// 設備名稱
        /// </summary>
        [JsonProperty("groupName")]
        public string GroupName { get; set; }
        /// <summary>
        /// 設備遙控裝置
        /// </summary>
        [JsonProperty("sensors")]
        public List<Sensor> Sensors { get; set; }
    }
}

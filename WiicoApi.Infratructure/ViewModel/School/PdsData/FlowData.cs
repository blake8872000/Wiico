using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
   public class FlowData
    {
        /// <summary>
        /// 流程編號
        /// </summary>
        [JsonProperty("flow_no")]
        /// 
        public string Flow_no { get; set; }
        /// <summary>
        /// 流程名稱
        /// </summary>
        [JsonProperty("flow_name")]
        public string Flow_name { get; set; }
        /// <summary>
        /// 通過狀態
        /// </summary>
        [JsonProperty("flow_status")]
        public PdsEnums? Flow_status { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberRequest : BackendBaseRequest
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
    }
}

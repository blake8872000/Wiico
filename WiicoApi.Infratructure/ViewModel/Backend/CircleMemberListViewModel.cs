using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberListViewModel
    {
        /// <summary>
        /// 列表
        /// </summary>
        [JsonProperty("list")]
        public IEnumerable<CircleMemberInfo> List { get; set; }
    }
}

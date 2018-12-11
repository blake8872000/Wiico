using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Property
{
    public class MiddleListResult<T> : ListResult<T> where T : IPagingData
    {

        [JsonProperty("newerResultToken")]
        public string NewerResultToken { get; set; }
        [JsonProperty("olderResultToken")]
        public string OlderResultToken { get; set; }
        
    }
}

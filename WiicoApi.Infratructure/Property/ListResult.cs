using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Property
{
    public class ListResult : SingleResult
    {

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }
        [JsonProperty("maxResult")]
        public int MaxResult { get; set; }
        
    }
}

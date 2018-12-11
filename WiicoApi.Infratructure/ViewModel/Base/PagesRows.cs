using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
    public class PagesRows
    {
        [JsonProperty("pages")]
        public int? Pages { get; set; }
        [JsonProperty("rows")]
        public int? Rows { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

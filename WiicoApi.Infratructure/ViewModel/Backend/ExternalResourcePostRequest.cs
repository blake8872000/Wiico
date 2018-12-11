using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class ExternalResourcePostRequest : BackendBaseRequest
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
        [JsonProperty("externalType")]
        public int ExternalType { get; set; }
        [JsonProperty("apis")]
        public List<ExternalResource> Apis { get; set; }
    }
}

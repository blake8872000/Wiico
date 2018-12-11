using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
    public class BaseViewModel
    {
        [JsonIgnore]
        public Guid EventId { get; set; }
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
    }
}

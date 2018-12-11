using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class ExtensionColumnValue
    {
        [JsonProperty("extensionColumnId")]
        public int ExtensionColumnId { get; set; }
        [JsonProperty("extensionColumnName")]
        public string ExtensionColumnName { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("dispalyName")]
        public string DispalyName { get; set; }
    }
}

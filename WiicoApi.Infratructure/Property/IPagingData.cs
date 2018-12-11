using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Property
{
    public interface IPagingData
    {
        [JsonIgnore]
        long RowNum { get; set; }
        [JsonProperty("id")]
        int Id { get; set; }
        Guid OuterKey { get; set; }
    }
}

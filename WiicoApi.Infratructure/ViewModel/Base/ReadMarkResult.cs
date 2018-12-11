
using Newtonsoft.Json;
using WiicoApi.Infrastructure.Property;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
    public class ReadMarkResult<T> : MiddleListResult<T> where T : IPagingData
    {
        [JsonProperty("readBegin")]
        public int ReadBegin { get; set; }

        [JsonProperty("readEnd")]
        public int ReadEnd { get; set; }
    }
}

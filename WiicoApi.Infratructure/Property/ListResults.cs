using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Property
{
    public class ListResult<T> : ListResult, IListResult<T> where T : IPagingData
    {

        [JsonProperty("data")]
        public T[] Data { get; set; }
            }
}

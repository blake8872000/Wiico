using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class FeedBackGetResponse
    {

        [JsonProperty("sumPages")]
        public int SumPages { get; set; }
        [JsonProperty("lastPage")]
        public int LastPage{ get; set; }
        [JsonProperty("nextPage")]
        public int NextPage { get; set; }
        [JsonProperty("feedbacks")]
        public List<Infrastructure.Entity.FeedBack> FeedBacks { get; set; }
    }
}

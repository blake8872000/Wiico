using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote
{
    public class VoteChangeStartRequest
    {
        [JsonProperty("userToken")]
       public Guid UserToken { get; set; }
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        [JsonProperty("isStart")]
        public bool IsStart { get; set; }
    }
}

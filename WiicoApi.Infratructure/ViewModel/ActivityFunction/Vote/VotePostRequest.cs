using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote
{
    public class VotePostRequest
    {
        [JsonProperty("outerKey")]
       public string OuterKey { get; set; }
        [JsonProperty("userToken")]
        public Guid UserToken { get; set; }
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("voteItems")]
        public List<Infrastructure.ViewModel.ActivityFunction.Vote.VoteItemViewModel> VoteItems { get; set; }
    }
}

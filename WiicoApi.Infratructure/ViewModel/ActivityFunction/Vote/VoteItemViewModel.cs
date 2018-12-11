using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote
{
    public class VoteItemViewModel
    {
        [JsonProperty("chooseContent")]
        public string ChooseContent { get; set; }
        [JsonProperty("chooseSort")]
        public int ChooseSort { get; set; }
        [JsonProperty("chooseName")]
        public string ChooseName { get; set; }
        [JsonProperty("chooseID")]
        public int? ChooseID { get; set; }
    }
}

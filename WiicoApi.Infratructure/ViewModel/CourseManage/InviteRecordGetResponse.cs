using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class InviteRecordGetResponse 
    {
        [JsonProperty("resType")]
        public int ResType { get; set; }
        [JsonProperty("useAccounts")]
        public List<string> UseAccounts { get; set; }
        [JsonProperty("unJoinAccounts")]
        public List<string> UnJoinAccounts { get; set; }
    }
}

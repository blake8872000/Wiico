using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class CourseManagerPostRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        [JsonProperty("inviteCode")]
        public string InviteCode { get; set; }
        [JsonProperty("resType")]
        public int ResType { get; set; }
        [JsonProperty("accounts")]
        public IEnumerable<string> Accounts { get; set; }
        
    }
}

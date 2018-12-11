using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class LearningCircleMemberInfo
    {
        [JsonProperty("memberId")]
        public int MemberId { get; set; }
        [JsonProperty("memberName")]
        public string MemberName { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
        [JsonProperty("accountId")]
        public string AccountId { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}

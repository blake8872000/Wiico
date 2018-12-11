using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberInfo
    {
        [JsonProperty("memberInfo")]
        public Infrastructure.Entity.Member MemberInfo { get; set; }
        [JsonProperty("learningCircleInfo")]
        public Infrastructure.Entity.LearningCircle LearningCircleInfo{ get; set; }
        [JsonProperty("learningRoleInfo")]
        public Entity.LearningRole LearningRoleInfo { get; set; }
    }
}

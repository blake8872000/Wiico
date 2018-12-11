using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class MemberInvitePutRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        [JsonProperty("enable")]
        public bool Enable{ get; set; }
        [JsonProperty("code")]
        public string InviteCode { get; set; }
        [JsonProperty("isCourseCode")]
        public bool IsCourseCode { get; set; }

    }
}

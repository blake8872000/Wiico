using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class MemberInvitePostRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        /// <summary>
        /// 要新增的角色代碼 0:成員 | 1:管理者
        /// </summary>
        [JsonProperty("inviteType")]
        public int InviteType { get; set; }
        [JsonProperty("inviteEmail")]
        public IEnumerable<string> InviteEmail { get; set; }
        [JsonProperty("ccToMe")]
        public bool CC { get; set; }
        
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    public class CircleMemberRoleManageGetResponse
    {

        [JsonIgnore]
        public int? ExternalRid { get; set; }
        [JsonProperty("isShowMail")]
        public bool IsShowMail { get; set; }
        [JsonProperty("roleId")]
        public int RoleId { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("isEdit")]
        public bool IsEdit { get; set; }
        /// <summary>
        /// 成員等級 越低權限越高
        /// </summary>
        [JsonProperty("level")]
        public int? Level { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// 加入方式  0: 系統後台管理 |1: 自行輸入邀請碼 | 2: 非Email管道取得邀請連結| 3: Email取得邀請連結
        /// </summary>
        [JsonProperty("resType")]
        public int? ResType { get; set; }

    }
}

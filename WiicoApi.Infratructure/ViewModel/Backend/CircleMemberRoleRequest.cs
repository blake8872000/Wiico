using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CircleMemberRoleRequest : BackendBaseRequest
    {
        [JsonProperty("roleId")]
        public int? RoleId { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [JsonProperty("accounts")]
        public List<string> Accounts { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        [JsonProperty("inviteCode")]
        public string InviteCode { get; set; }
        [JsonProperty("resType")]
        public int ResType { get; set; }
    }
}

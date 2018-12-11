using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class MemberInvite
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// 邀請對象 0:成員 | 1:管理者
        /// </summary>
        [JsonProperty("inviteType")]
        public int Type { get; set; }
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("inviteEmail")]
        public string InviteEmail { get; set; }
        [JsonProperty("isCourseCode")]
        public bool IsCourseCode { get; set; }
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        [NotMapped, JsonProperty("inviteUrl")]
        public string InviteUrl { get; set; }
    }
}
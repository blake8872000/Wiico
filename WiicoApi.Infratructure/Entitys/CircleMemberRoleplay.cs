using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// CircleMember表與Role表的關聯
    /// </summary>
    public class CircleMemberRoleplay
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleId")]
        public int CircleId { get; set; }
        /// <summary>
        /// 人物代碼
        /// </summary>
        [JsonProperty("memberId")]
        public int MemberId { get; set; }
        /// <summary>
        /// 角色代碼
        /// </summary>
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        [JsonProperty("externalRid")]
        public int? ExternalRid { get; set; }
        /// <summary>
        /// 加入方式  0 : 系統管理 | 1 : 邀請碼 | 2 :邀請連結 | 3 :Email
        /// </summary>
        [JsonProperty("resType")]
        public int? ResType { get; set; }
    }
}

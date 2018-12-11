using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{

    /// <summary>
    /// 組織角色資料表
    /// </summary>
    public class OrganizationRole
    {
        /// <summary>
        /// 組織角色編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 組織角色名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 是否為管理者
        /// </summary>
        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 組織角色代碼
        /// </summary>
        [JsonProperty("roleCode")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }

        /// <summary>
        /// 角色等級
        /// </summary>
        [JsonIgnore]
        public int Level { get; set; }
    }
}

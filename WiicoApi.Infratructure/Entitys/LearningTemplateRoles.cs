using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 組織課內範本角色
    /// </summary>
    public class LearningTemplateRoles
    {
        /// <summary>
        /// 範本角色編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 範本角色名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 範本角色等級
        /// </summary>
        [JsonProperty("level")]
        public int Level { get; set; }
        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }
        /// <summary>
        /// 範本角色代碼
        /// </summary>
        [JsonProperty("roleCode")]
        public string RoleKey { get; set; }

    }
}
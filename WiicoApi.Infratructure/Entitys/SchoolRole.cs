using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 組織課內角色範本資料表
    /// </summary>
    public class SchoolRole
    {
        /// <summary>
        /// 編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 角色名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }
        /// <summary>
        /// 角色代碼
        /// </summary>
        [JsonProperty("RoleCode")]
        public string RoleKey { get; set; }
        /// <summary>
        /// 角色等級  1 : 最高 可編輯多資料 |  2 : 中等 編輯少量資料  | 3 : 最低 沒有編輯權限
        /// </summary>
        [JsonIgnore]
        public int Level { get; set; }
    }
}

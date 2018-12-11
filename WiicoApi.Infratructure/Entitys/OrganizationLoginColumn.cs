using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 組織登入API的參數
    /// </summary>
    public class OrganizationLoginColumn
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 欄位名稱 
        /// </summary>
        [JsonProperty("columnKey")]
        public string ColumnKey { get; set; }
        /// <summary>
        /// 丟API的方法 - Get、Post、Put
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        /// 欄位代表屬性 - account、pwd、deviceId
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// 所屬組織
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }
    }
}

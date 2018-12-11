using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 行事曆組織角色關聯資料表
    /// </summary>
    public class CalendarOrganizationRole
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 行事曆編號
        /// </summary>
        [JsonProperty("calendarId")]
        public int CalendarId { get; set; }
        /// <summary>
        /// 組織角色編號
        /// </summary>
        [JsonProperty("organizationRoleId")]
        public int OrganizationRoleId { get; set; }
    }
}

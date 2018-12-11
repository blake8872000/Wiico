using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 行事曆與學院關聯
    /// </summary>
    public class CalendarDept
    {
        /// <summary>
        /// 關聯編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 行事曆編號
        /// </summary>
        [JsonProperty("calendarId")]
        public int CalendarId { get; set; }
        /// <summary>
        /// 分類院系編號
        /// </summary>
        [JsonProperty("deptId")]
        public int DeptId { get; set; }
    }
}

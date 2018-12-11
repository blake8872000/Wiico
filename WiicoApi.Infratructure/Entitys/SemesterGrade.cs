using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 課程期別資訊
    /// </summary>
    public partial class SemesterGrade
    {
        /// <summary>
        /// 學制編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 學制名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 學制要念幾年
        /// </summary>
        [JsonProperty("gradeYears")]
        public int GradeYears { get; set; }
        /// <summary>
        /// 學制代碼
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 組織編號 
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }
    }
}

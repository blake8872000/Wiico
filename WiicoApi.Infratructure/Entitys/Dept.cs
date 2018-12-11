using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WiicoApi.Infrastructure;
namespace WiicoApi.Infrastructure.Entity
{
    public class Dept :Base.EntityBase
    {
        /// <summary>
        /// 部門/系所名稱
        /// </summary>
        [JsonProperty("name")]
        [MaxLength(150)]
        [Display(Name = "DeptName", ResourceType = typeof(Localization))]
        public override string Name { get; set; }

        /// <summary>
        /// 部門/系所名稱縮寫
        /// </summary>
        [JsonProperty("shortName")]
        public string ShortName { get; set; }
        /// <summary>
        /// 組織/學校編號
        /// </summary>
        [JsonProperty("ordId")]
        public int? OrgId { get; set; }
        /// <summary>
        /// 上層部門/系所編號
        /// </summary>
        [JsonProperty("parentId")]
        public int? ParentId { get; set; }
        /// <summary>
        /// 部門/系所代碼
        /// </summary>
        [JsonProperty("deptCode")]
        [MaxLength(50)]
        public string DeptCode { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 是否允許顯示
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }


        public virtual Dept Parent { get; set; }
 
    }
}

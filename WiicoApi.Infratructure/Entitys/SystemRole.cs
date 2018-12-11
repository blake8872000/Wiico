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
    /// 屬於系統的角色資料總表
    /// </summary>
    public class SystemRole : Base.EntityBase
    {
        /// <summary>
        /// 系統角色名稱
        /// </summary>
        [MaxLength(50),JsonProperty("name")]
        public override string Name { get; set; }

        /// <summary>
        /// 組織/學校編號 - 流水號
        /// </summary>
        //public int OrgId { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
       [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// 角色類別 - 10：系統管理員  20：自訂身分 30：自訂身分
        /// </summary>
        [JsonProperty("roleType")]
        public string RoleType { get; set; }
        /// <summary>
        /// 是否為管理員
        /// </summary>
        //  public bool IsAdmin { get; set; }
        /// <summary>
        /// 是否為系統管理員
        /// </summary>
        [JsonProperty("isSystemManager")]
        public bool IsSystemManager { get; set; }
    }
}

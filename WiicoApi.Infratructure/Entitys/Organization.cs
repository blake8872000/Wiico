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
    /// <summary>
    /// 組織/學校的資料表
    /// </summary>
    public class Organization : Base.EntityBase
    {
        /// <summary>
        /// 組織/學校名稱
        /// </summary>
        [JsonProperty("name")]
        [MaxLength(300)]
        [Display(Name = "OrganizationName", ResourceType = typeof(Localization))]
        public override string Name { get; set; }

        /// <summary>
        /// 組織/學校代碼 - 英文
        /// </summary>
        [JsonProperty("orgCode")]
        [MaxLength(100)]
        public string OrgCode { get; set; }

        /// <summary>
        /// 組織/學校關聯資料庫的編號 - 流水號 [SqlConnectionString資料表的Id]
        /// </summary>
        [JsonIgnore]
        public int ConnId { get; set; }

        ///// <summary>
        ///// 啟用/停用
        ///// </summary>
        //public bool Enable { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }

        /// <summary>
        ///用於組織[學校]介接api的驗證碼
        /// </summary>
        [JsonProperty("apiKey")]
        public string APIKey { get; set; }
        /// <summary>
        /// 學期總數
        /// </summary>
        [JsonProperty("semesterLength")]
        public int? SemesterLength { get; set; }

        [JsonProperty("isOrgRegister")]
        public bool IsOrgRegister { get; set; }
    }
}

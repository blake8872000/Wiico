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
    /// 學期/梯次資料
    /// </summary>
    public class Section : Base.EntityBase
    {
        /// <summary>
        /// 學期/梯次名稱
        /// </summary>
        [JsonProperty("name")]
        [MaxLength(10)]
        public override string Name { get; set; }

        /// <summary>
        /// 學期/梯次完整名稱
        /// 期別年度
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("serial")]
        public int? Serial { get; set; }

        /// <summary>
        /// 學年 - 西元年
        /// </summary>
        [JsonProperty("year")]
        public int Year { get; set; }
        /// <summary>
        /// 組織代碼
        /// </summary>
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }

        /// <summary>
        /// 是否為當學期
        /// </summary>
        [JsonProperty("isNowSeme")]
        public bool IsNowSeme { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
    }
}

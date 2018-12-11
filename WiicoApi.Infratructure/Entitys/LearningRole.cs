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
    /// 學習圈角色資料表
    /// </summary>
    public class LearningRole : Base.EntityBase
    {
        /// <summary>
        /// 學習圈角色名稱
        /// </summary>
        [MaxLength(50)]
        [JsonProperty("name")]
        public override string Name { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// 角色類別- 10：系統管理員  20：固定身分(學生、老師) 30：自訂身分
        /// </summary>
        [JsonProperty("roleType")]
        [MaxLength(2)]
        public string RoleType { get; set; }

        /// <summary>
        /// 是否為固定身分
        /// </summary>
        [JsonProperty("isFixed")]
        public bool IsFixed { get; set; }

        /// <summary>
        /// 是否為管理者
        /// </summary>
        [JsonProperty("isAdminRole")]
        public bool IsAdminRole { get; set; }

        /// <summary>
        /// 取得ican5的 manscore_type - 同步member用
        /// </summary>
        [JsonProperty("ican5Memo")]
        public string Ican5Memo { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        [JsonProperty("externalRid")]
        public int? ExternalRid { get; set; }
        /// <summary>
        /// 角色排序
        /// </summary>
        [JsonProperty("sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 角色等級 1: 可做課內編輯 | 2:可做部分課內編輯 | 3:不能做任何編輯(參與者)
        /// </summary>
        [JsonProperty("level")]

        public int? Level { get; set; }
    }
}

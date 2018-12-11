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
    public class LearningCircle : Base.EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        [MaxLength(50)]
        public override string Name
        {
            get; set;
        }


        /// <summary>
        /// 模組名稱
        /// </summary>
        [JsonProperty("moduleName")]
        public string ModuleName { get; set; }


        /// <summary>
        /// 期別
        /// </summary>
        [JsonProperty("section")]
        [MaxLength(10)]
        public string Section { get; set; }
        /// <summary>
        /// 課程代碼 - 流水號
        /// </summary>
        [JsonProperty("courseId")]
        public int? CourseId { get; set; }
        public virtual Course Course { get; set; }
        /// <summary>
        /// 學習圈類型 - 10 : 課程學習圈 ; 20 : 分組 ; 30 : 自訂
        /// </summary>
        [JsonProperty("lCType")]
        public int LCType { get; set; }


        /// <summary>
        /// GoogleDriveFieldUrl 
        /// </summary>
        [JsonProperty("googleDriveFielUrl")]
        public string GoogleDriveFielUrl { get; set; }


        /// <summary>
        /// 提供其他系統呼叫的編碼key
        /// </summary>
        [JsonProperty("learningOuterKey")]
        [MaxLength(50)]
        [Display(Name = "LearningCircleOuterKey", ResourceType = typeof(Localization))]
        public string LearningOuterKey { get; set; }

        /// <summary>
        /// 課程描述
        /// </summary>
        [Display(Name = "LearningCircleDescription", ResourceType = typeof(Localization))]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Display(Name = "LearningCircleEnable", ResourceType = typeof(Localization))]
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        [JsonProperty("externalRid")]
        public int? ExternalRid { get; set; }

        /// <summary>
        /// 是否允許顯示
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }

        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 所屬組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }

        /// <summary>
        /// 教學目標
        /// </summary>
        [JsonProperty("objective")]
        public string Objective { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [JsonProperty("reMark")]
        public string ReMark { get; set; }
        /// <summary>
        /// 邀請碼開關
        /// </summary>
        [JsonProperty("inviteEnable")]
        public bool InviteEnable { get; set; }
        /// <summary>
        /// 管理者邀請碼開關
        /// </summary>
        [JsonProperty("adminInviteEnable")]
        public bool AdminInviteEnable { get; set; }
    }
}

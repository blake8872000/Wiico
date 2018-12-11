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
    /// 人員帳號 基本資料表
    /// </summary>
    public class Member : Base.EntityBase
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "MemberName", ResourceType = typeof(Localization))]
        [JsonProperty("name")]
        public override string Name { get; set; }

        /// <summary>
        /// 隸屬組織/學校編號 - 流水號
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [JsonProperty("account")]
        [MaxLength(100)]
        [Display(Name = "MemberAccount", ResourceType = typeof(Localization)) ]
        
        public string Account { get; set; }

        
        [ScaffoldColumn(false)]
        [Display(Name = "MemberPassword", ResourceType = typeof(Localization)) ]
        /// <summary>
        /// 密碼
        /// </summary>
        [JsonProperty("passWord")]
        public byte[] PassWord { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        [JsonProperty("email")]
        [MaxLength(100)]
        [Display(Name = "Member_Email", ResourceType = typeof(Localization))]
        public string Email { get; set; }

        /// <summary>
        /// 照片連結
        /// </summary>
        [JsonProperty("photo")]
        [MaxLength(500)]
        [Display(Name = "MemberPhoto", ResourceType = typeof(Localization))]
        public string Photo { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        [Display(Name = "MemberEnable", ResourceType = typeof(Localization))]
        public bool Enable { get; set; }

        /// <summary>
        /// 是否顯示-ui看不到，若刪除人員此值設定為False
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }

        /// <summary>
        /// 是否為外部系統匯入之人員 - 有值代表是外部，預設為NULL
        /// </summary>
        [JsonProperty("externalRid")]
        public int? ExternalRid { get; set; }

        /// <summary>
        /// 是否為組織管理員
        /// </summary>
        [JsonProperty("isOrgAdmin")]
        public bool IsOrgAdmin { get; set; }

        /// <summary>
        /// 是否公開email
        /// </summary>
        [JsonProperty("isShowEmail")]
        public bool IsShowEmail { get; set; }

        /// <summary>
        /// SignalRConnectionId
        /// </summary>
        [JsonProperty("connectionId")]
        public string ConnectionId { get; set; }

        /// <summary>
        /// 所屬院系編號 2017-09-27 yushuchen新增
        /// </summary>
        [JsonProperty("deptId")]
        public int? DeptId { get; set; }

        /// <summary>
        /// 所屬年級 2017-09-27 yushuchen新增
        /// </summary>
        [JsonProperty("grade")]
        public int? Grade { get; set; }

        /// <summary>
        /// 入學年 - 學籍 1061 2017-09-27 yushuchen新增
        /// </summary>
        [JsonProperty("schoolRoll")]
        public string SchoolRoll { get; set; }

        /// <summary>
        /// 畢業狀態 -10:在學 20:畢業 30:休學 40:退學 2017-09-27 yushuchen新增
        /// </summary>
        [JsonProperty("graduationStatus")]
        public int? GraduationStatus { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        /// <summary>
        /// 學制編號
        /// </summary>
        [JsonProperty("semesterGradeId")]
        public int? SemesterGradeId { get; set; }
        /// <summary>
        /// 開班年級
        /// </summary>
        [JsonProperty("classGrade")]
        public int? ClassGrade { get; set; }

        /// <summary>
        /// 模組代碼
        /// </summary>
        [JsonProperty("idMainDoma")]
        public string IDMainDoma { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
        [JsonProperty("nameMainDoma")]
        public string NameMainDoma { get; set; }

        /// <summary>
        /// 組織角色編號
        /// </summary>
        [JsonProperty("organizationRoleId")]
        public int? OrganizationRoleId { get; set; }

        /// <summary>
        /// 已被驗證
        /// </summary>
        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }
}

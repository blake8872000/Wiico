using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class UserGetResponseData
    {
        public int Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 隸屬組織/學校編號 - 流水號
        /// </summary>
        public int OrgId { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>


        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [JsonIgnore]
        public byte[] Password { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>

        public string Email { get; set; }

        /// <summary>
        /// 照片連結
        /// </summary>

        public string Photo { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否顯示-ui看不到，若刪除人員此值設定為False
        /// </summary>
        public bool Visibility { get; set; }

        /// <summary>
        /// 是否為外部系統匯入之人員 - 有值代表是外部，預設為NULL
        /// </summary>
        public int? ExternalRid { get; set; }

        /// <summary>
        /// 是否為組織管理員
        /// </summary>
        public bool IsOrgAdmin { get; set; }

        /// <summary>
        /// 是否公開email
        /// </summary>
        public bool IsShowEmail { get; set; }

        /// <summary>
        /// SignalRConnectionId
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 所屬院系編號 2017-09-27 yushuchen新增
        /// </summary>
        public int? DeptId { get; set; }

        /// <summary>
        /// 所屬年級 2017-09-27 yushuchen新增
        /// </summary>
        public int? Grade { get; set; }

        /// <summary>
        /// 入學年 - 學籍 1061 2017-09-27 yushuchen新增
        /// </summary>
        public string SchoolRoll { get; set; }

        /// <summary>
        /// 畢業狀態 -10:在學 20:畢業 30:休學 40:退學 2017-09-27 yushuchen新增
        /// </summary>
        public int? GraduationStatus { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 學制編號
        /// </summary>
        public int? SemesterGradeId { get; set; }
        /// <summary>
        /// 開班年級
        /// </summary>
        public int? ClassGrade { get; set; }

        /// <summary>
        /// 模組代碼
        /// </summary>
        public string IDMainDoma { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
        public string NameMainDoma { get; set; }

        /// <summary>
        /// 組織角色編號
        /// </summary>
        public int? OrganizationRoleId { get; set; }

        /// <summary>
        /// 已被驗證
        /// </summary>
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [NotMapped]
        public UserGetResponseExtraInfo ExtraInfo { get; set; }


    }
}

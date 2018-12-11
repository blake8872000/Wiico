using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class PersonViewModel
    {
        /// <summary>
        /// 學生代號 - 帳號
        /// </summary>
        [JsonProperty("studId")]
        public string Account { get; set; }

        /// <summary>
        /// 學生姓名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        [JsonProperty("photo")]
        public string Photo { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// 所屬學院
        /// </summary>
        [JsonProperty("collegeName")]
        public string CollegeName { get; set; }

        /// <summary>
        /// 所屬學院縮寫
        /// </summary>
        [JsonProperty("collegeBriefName")]
        public string CollegeBriefName { get; set; }

        /// <summary>
        /// 學生年級
        /// </summary>
        [JsonProperty("Stud_Grade")]
        public int? Grade { get; set; }

        /// <summary>
        /// 學生入學年
        /// </summary>
        [JsonProperty("Stud_SchInYear")]
        public string SchoolRoll { get; set; }

        /// <summary>
        /// 畢業狀態
        /// </summary>
        [JsonIgnore]
        public int? GraduationStatus { get; set; }

        /// <summary>
        /// 是否顯示信箱
        /// </summary>
        [JsonProperty("isShowEmail")]
        public bool IsShowEmail { get; set; }

        /// <summary>
        /// 顯示畢業狀態
        /// </summary>
        [JsonProperty("studStatus")]
        public string StudStatus { get; set; }
        /// <summary>
        /// 所屬學院編號
        /// </summary>
        [JsonIgnore]
        public int? DeptId { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        /// <summary>
        /// 主修模組名稱
        /// </summary>
        [JsonProperty("mainDomain")]
        public string MainDomain { get; set; }

        /// <summary>
        /// 學級
        /// </summary>
        [JsonProperty("SemeGrade")]
        public string SemeGrade { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    /// <summary>
    /// 暫存成員資訊
    /// </summary>
    public class TempMember
    {
        /// <summary>
        /// 成員編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 成員名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 成員所屬單位
        /// </summary>
        public int OrgId { get; set; }
        /// <summary>
        /// 成員信箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 成員帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 成員相簿
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 成員是否有效
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 成員角色名稱
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 是否為管理員
        /// </summary>
        public bool IsAdminRole { get; set; }
        /// <summary>
        /// 學習圈編號
        /// </summary>
        public int LearningId { get; set; }
        /// <summary>
        /// 角色排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 是否顯示email
        /// </summary>
        public bool IsShowEmail { get; set; }
        /// <summary>
        /// 課程模組名稱
        /// </summary>
        public string DomainName { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 學年級
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 系所名稱 - 縮寫
        /// </summary>
        public string CollegeSName { get; set; }
        /// <summary>
        /// 系所名稱 - 全部名稱
        /// </summary>
        public string CollegeName { get; set; }
    }
}

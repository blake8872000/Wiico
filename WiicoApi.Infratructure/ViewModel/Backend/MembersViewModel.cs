using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class MembersViewModel
    {
        /// 組織代號
        /// </summary>
        public int OrgId { get; set; }

        /// <summary>
        /// 組織名稱
        /// </summary>
        [Display(Name = "OrganizationName", ResourceType = typeof(Localization))]
        public string OrgName { get; set; }
        /// <summary>
        /// 人員基本資料
        /// </summary>
        public Member MemberInfo { get; set; }
    }
}

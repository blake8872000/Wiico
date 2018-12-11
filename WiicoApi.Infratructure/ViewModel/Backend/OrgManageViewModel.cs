using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 顯示組織管理者頁面
    /// </summary>
    public class OrgManageViewModel
    {
        public Organization OrgInfo { get; set; }

        public List<Member> Members { get; set; }

        public List<Member> OrgManagers { get; set; }
    }
}

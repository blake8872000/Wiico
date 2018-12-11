using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 取得組織成員列表 
    /// </summary>
    public class OrganizationMemberViewModel
    {
        public Organization OrgInfo;

        public Member Member;

        public IList<Member> MemberInfo;
    }
}

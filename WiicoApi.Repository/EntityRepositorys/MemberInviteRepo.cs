using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class MemberInviteRepo : GenericEntityRepository<MemberInvite,WiicoDB>
    {
        public MemberInviteRepo(WiicoDB context) : base(context)
        { }
        public IEnumerable<MemberInvite> GetList(string circleKey,bool? isJoin,int inviteType) {
            var orgnaizationInfo = (from o in _context.Organizations
                                    join lc in _context.LearningCircle on o.Id equals lc.OrgId
                                    where lc.LearningOuterKey == circleKey
                                    select o).FirstOrDefault();
            if (orgnaizationInfo == null)
                return null;
            var responseData =( from mi in _context.MemberInvite
                               join lc in _context.LearningCircle on mi.CircleKey equals lc.LearningOuterKey
                               where lc.LearningOuterKey==circleKey && mi.Type== inviteType
                                select mi);
            if(responseData.FirstOrDefault()==null)
                return null;
            //判斷是否
            if (isJoin.HasValue && isJoin.Value)
                responseData = responseData.Where(t => t.Enable == true);
            else if (isJoin.HasValue && isJoin.Value == false)
                responseData = responseData.Where(t => t.Enable == false);
            foreach (var inviteInfo in responseData.ToList()) {
                inviteInfo.CreateDate = inviteInfo.CreateDate.ToLocalTime();
                inviteInfo.InviteUrl = string.Format("http://scedev.eastus.cloudapp.azure.com/FlipusWeb/Invite?r=2&o={0}&c={1}&t{2}", orgnaizationInfo.OrgCode, inviteInfo.Code,inviteType);
            }

            return responseData.ToList();
        }
    }
}

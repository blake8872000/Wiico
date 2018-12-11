using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityRepository;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class CircleMemberRoleplaysRepo : GenericEntityRepository<CircleMemberRoleplay,WiicoDB>
    {

        public CircleMemberRoleplaysRepo(WiicoDB context) : base(context)
        {
        }

        

        public IQueryable<CircleMemberRoleplay> GetDataByMemberIdCircleKey(int memberId, string circleKey) {
            var data = (from cmr in _context.CircleMemberRoleplay
                        join lc in _context.LearningCircle on cmr.CircleId equals lc.Id
                        where lc.LearningOuterKey.ToLower() == circleKey.ToLower() &&
                                     cmr.MemberId == memberId
                        select cmr);
            return data;
        }

        public IQueryable<CircleMemberRoleplay> GetDataByAccountCircleKey(string account, string circleKey)
        {
            var data = (from cmr in _context.CircleMemberRoleplay
                        join lc in _context.LearningCircle on cmr.CircleId equals lc.Id
                        join m in _context.Members on cmr.MemberId equals m.Id
                        where lc.LearningOuterKey.ToLower() == circleKey.ToLower() &&
                                     m.Account.ToLower() == account 
                        select cmr);
            return data;
        }

        public IQueryable<CircleMemberRoleplay> GetDataByMemberIdCircleKeyRoleId(int memberId, string circleKey, int roleId)
        {
            var data = (from cmr in _context.CircleMemberRoleplay
                        join lc in _context.LearningCircle on cmr.CircleId equals lc.Id
                        where lc.LearningOuterKey.ToLower() == circleKey.ToLower() &&
                                     cmr.MemberId == memberId &&
                                     cmr.RoleId == roleId
                        select cmr);
            return data;
        }

        public IQueryable<CircleMemberRoleplay> GetDataByAccountCircleKeyRoleId(string account,string circleKey,int roleId) {
            var data = (from cmr in _context.CircleMemberRoleplay
                        join lc in _context.LearningCircle on cmr.CircleId equals lc.Id
                        join m in _context.Members on cmr.MemberId equals m.Id
                        where lc.LearningOuterKey.ToLower() == circleKey.ToLower() &&
                                     m.Account == account &&
                                     cmr.RoleId == roleId
                        select cmr);
            return data;
        }

        public IEnumerable<GroupMember> GetGroupMember(int learningId, int categoryId)
        {
            var sql = @"
         select cm.MemberId as Id , mv.Account as Account, mv.Name as Name, mv.Photo as Photo
                        from [dbo].[CircleMemberRoleplays] cm
                        inner join [dbo].[MembersView] mv on cm.MemberId = mv.Id
						inner join [dbo].[LearningRoles] lr on cm.CircleId = lr.LearningId and cm.RoleId = lr.Id
                        where cm.CircleId = @learningId and lr.Ican5Memo like '10%'
                        ";


            var circleMembers = _context.Database.SqlQuery<GroupMember>(sql,
                         new SqlParameter("@learningId", learningId),
                         new SqlParameter("@categoryId", categoryId)).ToList();

            return circleMembers;
        }
    }
}

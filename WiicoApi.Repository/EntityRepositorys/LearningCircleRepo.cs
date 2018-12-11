using EntityRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class LearningCircleRepo : GenericEntityRepository<LearningCircle, WiicoDB>
    {
        public LearningCircleRepo(WiicoDB context) : base(context)
        { }

        /// <summary>
        /// 根據使用者編號取得該使用者的課程列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<LearningCircle> GetListByMemberId(int memberId) {

            var responseData = (from lc in _context.LearningCircle
                                join cmr in _context.CircleMemberRoleplay on lc.Id equals cmr.CircleId
                                where cmr.MemberId == memberId
                                select lc).ToList();
            if (responseData.FirstOrDefault() == null)
                return null;
            else
                return responseData;
        }
        /// <summary>
        /// 根據使用者登入代碼取得該使用者的課程列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<LearningCircle> GetListByUserToken(string token)
        {
            var responseData = (from lc in _context.LearningCircle
                                join cmr in _context.CircleMemberRoleplay on lc.Id equals cmr.CircleId
                                join ut in _context.UserToken on cmr.MemberId equals ut.MemberId
                                where ut.Token.ToLower()==token.ToLower()
                                select lc).ToList();
            if (responseData.FirstOrDefault() == null)
                return null;
            else
                return responseData;
        }
    }
}

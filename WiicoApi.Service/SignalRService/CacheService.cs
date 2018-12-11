using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using WiicoApi.Infrastructure.BusinessObject;
using WiicoApi.Repository;

namespace WiicoApi.Service.SignalRService
{
    public class CacheService
    {
        private readonly GenericUnitOfWork _uow;

        public CacheService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得學習圈所有成員
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public List<string> GetCircleMemberList(string circleKey, int? myId)
        {
            // 從快取取出學習圈成員列表
            var data = GetCircleMember(circleKey);

            // 2016-9-20 add by sophiee:APP team告知，推播未註明接收對象的事件，代表發給該課程中的所有成員(除了自己)
            // 因此增加myId參數，如果有傳值，就特別排除掉自己
            if (myId.HasValue)
            {
                data = data.Where(x => x.Id != myId).ToList();
            }

            return data.Select(x => x.Account).ToList();
        }
        /// <summary>
        /// 取出學習圈紀錄
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.CircleCacheData GetCircle(string circleKey)
        {
            string cacheKey = circleKey + "circleData";

            // 從快取取出學習圈紀錄
            var data = HttpContext.Current.Cache.Get(cacheKey) as CircleCacheData;

            if (data == null)
            {
                // 從資料庫取得學習圈資料
                var Rep = _uow.EntityRepository<Infrastructure.Entity.LearningCircle>();
                var circle = Rep.GetFirst(x => x.LearningOuterKey == circleKey);
                if (circle != null)
                    data = new CircleCacheData() { Id = circle.Id, Name = circle.Name };
                else
                    data = new CircleCacheData();

                HttpContext.Current.Cache.Insert(cacheKey, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }

            return data;
        }

        public List<Infrastructure.BusinessObject.MemberCacheData> GetCircleTeacherMember(string circleKey)
        {
            string cacheKey = circleKey + "circleMembers";

            // 從快取取出學習圈成員列表
            var data = HttpContext.Current.Cache.Get(cacheKey) as List<MemberCacheData>;

            if (data == null)
            {
                // 從資料庫取得帳號列表
                var db = _uow.DbContext;
                data = (from c in db.LearningCircle
                        join cm in db.CircleMember on c.Id equals cm.CircleId
                        join cmr in db.CircleMemberRoleplay on c.Id equals cmr.CircleId
                        join m in db.Members on cm.MemberId equals m.Id
                        join lr in db.LearningRole on cmr.RoleId equals lr.Id
                        where c.LearningOuterKey == circleKey && c.Enable == true && lr.IsAdminRole == true
                        select new MemberCacheData { Id = m.Id, Account = m.Account, ConnectionId = m.ConnectionId }).ToList();

                HttpContext.Current.Cache.Insert(cacheKey, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }

            return data;
        }

        public List<Infrastructure.BusinessObject.MemberCacheData> GetCircleMember(string circleKey)
        {
            string cacheKey = circleKey + "circleMembers";

            if (HttpContext.Current ==null) {
                return null;
            }
            // 從快取取出學習圈成員列表
            var data = HttpContext.Current.Cache.Get(cacheKey) as List<MemberCacheData>;

            if (data == null)
            {
                // 從資料庫取得帳號列表
                var db = _uow.DbContext;
                data = (from c in db.LearningCircle
                        join cm in db.CircleMemberRoleplay on c.Id equals cm.CircleId
                        join m in db.Members on cm.MemberId equals m.Id
                        where c.LearningOuterKey == circleKey && c.Enable == true
                        select new MemberCacheData { Id = m.Id, Account = m.Account, ConnectionId = m.ConnectionId }).ToList();

                HttpContext.Current.Cache.Insert(cacheKey, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }

            return data;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SignalRService
{
    public class SignalrService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly ActivityService activityService = new ActivityService();
        private readonly LearningCircleService learningCircleService = new LearningCircleService();
        private readonly CacheService cacheService = new CacheService();
        private readonly MemberService memberService = new MemberService();
        private readonly NoticeService noticeService = new NoticeService();

        public SignalrService()
        {
            _uow = new GenericUnitOfWork();
        }

        public string GetConnectId(int memberId)
        {
            var member = _uow.DbContext.Members.Find(memberId);

            if (member != null)
            {
                var connectId = member.ConnectionId;
                if (connectId != null)
                    return connectId;
            }

            return null;
        }

        /// <summary>
        /// 查詢學習圈底下 ConnectId 和最後活動紀錄
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="version">true 新版(全) / old 舊版(扣除自己) </param>
        /// <returns></returns>
        public Dictionary<List<string>, dynamic> GetConnectIdAndData(string circleKey, int memberId, SignalrConnectIdType connectIdType, SignalrDataType dataType)
        {
            Dictionary<List<string>, dynamic> dic = new Dictionary<List<string>, dynamic>();

            List<int> membersId = new List<int>();
            var learningCircleService = new LearningCircleService();
            var learningcircleInfo = learningCircleService.GetDetailByOuterKey(circleKey.ToLower());
            if (learningcircleInfo == null)
                return dic;
            switch (connectIdType)
            {
                case SignalrConnectIdType.All:
                    var cacheMembers = cacheService.GetCircleMember(circleKey);
                    if (cacheMembers == null)
                        return dic;
                    foreach (var member in cacheMembers)
                    {
                        membersId.Add(member.Id);
                    }
                    break;
                case SignalrConnectIdType.Other:
                    var cacheOtherMembers = cacheService.GetCircleMember(circleKey).Where(x => x.Id != memberId);
                    if (cacheOtherMembers == null || cacheOtherMembers.FirstOrDefault()==null)
                        return dic;
                    foreach (var member in cacheOtherMembers.ToList())
                    {
                        membersId.Add(member.Id);
                    }
                    break;
                case SignalrConnectIdType.Teachers:
                    var membersAccount = learningCircleService.GetCircleTeacherListBySql(circleKey, memberId);
                    if (membersAccount == null)
                        return dic;
                    foreach (var account in membersAccount)
                    {
                        membersId.Add(memberService.AccountToMember(account, learningcircleInfo.OrgId.Value).Id);
                    }
                    break;
                case SignalrConnectIdType.One:
                    membersId.Add(memberId);
                    break;
            }

            foreach (var member in membersId)
            {
                if (HttpContext.Current == null)
                    return dic;
                if (HttpContext.Current.Cache.Get(member.ToString()) is List<string> myConn)
                {
                    var data = GetData(dataType, circleKey, member);
                    dynamic _value;
                    if (!dic.TryGetValue(myConn, out _value))
                        dic.Add(myConn, data);

                }
            }

            return dic;
        }

        private dynamic GetData(SignalrDataType signalrDataType, string circleKey, int memberId)
        {
            /* 2018-04-02 yuschang
             */
            switch (signalrDataType)
            {
                case SignalrDataType.Notice:
                    return noticeService.GetNoticeList(circleKey, memberId, 1);
                case SignalrDataType.Activity:
                    return activityService.GetLatestList(memberId, "");
                default:
                    return null;
            }
        }
    }
}

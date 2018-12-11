using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Repository;

namespace WiicoApi.Service.SignalRService
{
    public class LikeService
    {
        private readonly GenericUnitOfWork _uow;

        public LikeService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 塞讚資訊 - [各模組 + 留言 的讚]
        /// </summary>
        /// <param name="eventId">點讚的eventId</param>
        /// <param name="_eventName">點讚對象</param>
        /// <param name="isMsg">點讚對象是否為留言</param>
        /// <param name="memberId">目前登入者代碼</param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.LikeCount CheckLikeInfo(Guid eventId, string _eventName, bool isMsg, int memberId)
        {
            var db = _uow.DbContext;
            var _actModuleMsgRep = _uow.EntityRepository<ActModuleMessage>();
            //點讚資訊
            var _actLikeRep = _uow.EntityRepository<LikeLog>();
            var alr = _actLikeRep.Get(t => t.OuterKey.Equals(eventId));
            var likeArray = from m in db.Members
                            join ll in db.LikeLog on m.Id equals ll.MemberId
                            where ll.OuterKey == eventId
                            select m.Account;
            var result = new Infrastructure.BusinessObject.LikeCount();
            result.EventId = eventId;
            //如果是模組活動本身的讚
            if (!isMsg)
            {
                result.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(eventId);
            }
            //如果是活動底下留言的讚
            else
            {
                //留言資訊
                var _msgInfo = _actModuleMsgRep.GetFirst(t => t.OuterKey == (eventId));
                //所屬模組 - 各模組做 discussion
                switch (_msgInfo.ModuleType)
                {
                    case "discussion":
                        //查詢該留言所屬活動代碼
                        var _discussionMsgRep = _uow.EntityRepository<ActDiscussionMsg>();
                        var _discussionRep = _uow.EntityRepository<ActDiscussion>();
                        var _discussionMsgInfo = _discussionMsgRep.GetFirst(t => t.OuterKey == _msgInfo.OuterKey);
                        var _discussionInfo = _discussionRep.GetFirst(t => t.Id == _discussionMsgInfo.ActDiscussionId);
                        result.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(_discussionInfo.EventId);
                        break;
                }
            }

            if (alr.Any())
            {
                var isLiked = false;
                //判斷是否點過讚
                foreach (var _item in alr)
                {
                    if (_item.MemberId.Equals(memberId))
                    {
                        isLiked = true;
                    }
                }
                result.EventName = _eventName;
                result.IsLiked = isLiked;
                result.IsMessage = isMsg;
                result.LikeInfo = alr.ToList();
                result.LikeArray = likeArray.ToList();
            }
            return result;
        }
    }
}


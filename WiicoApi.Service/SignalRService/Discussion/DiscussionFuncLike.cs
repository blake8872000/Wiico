using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Repository;

namespace WiicoApi.Service.SignalRService.Discussion
{
    public class DiscussionFuncLike
    {
        private readonly GenericUnitOfWork _uow = new GenericUnitOfWork();
        private object lockObject = new object();

        /// <summary>
        /// 取得按讚資訊
        /// </summary>
        /// <param name="outerKey">活動代碼/留言代碼</param>
        /// <returns></returns>
        public List<string> GetLikeArrayByOuterKey(string outerKey)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (eventId.HasValue)
                return GetLikeArrayByEventId(eventId.Value);
            else
                return new List<string>();
        }
        /// <summary>
        /// 取得按讚資訊
        /// </summary>
        /// <param name="eventId">活動代碼/留言代碼</param>
        /// <returns></returns>
        public List<string> GetLikeArrayByEventId(Guid eventId)
        {
            var db = _uow.DbContext;
            var sqlData = from m in db.Members
                          join l in db.LikeLog on m.Id equals l.MemberId
                          where l.OuterKey == eventId && l.DeleteUser == null
                          select m.Account;
            if (sqlData.FirstOrDefault() == null)
                return new List<string>();
            return sqlData.ToList();
        }

        /// <summary>
        /// 變更按讚狀態
        /// </summary>
        /// <param name="memberInfo">按讚者資訊</param>
        /// <param name="activityOuterKey">主題討論代碼</param>
        /// <param name="commentOuterKey">留言代碼 / 回覆代碼</param>
        /// <returns></returns>
        public DiscussionUpdateLikeInfo SwitchLike(Member memberInfo, string activityOuterKey, string commentOuterKey)
        {
            lock (lockObject)
            {
                var db = _uow.DbContext;
                var result = new DiscussionUpdateLikeInfo();
                //主題討論的代碼
                var eventId = Utility.OuterKeyHelper.CheckOuterKey(activityOuterKey);
                if (eventId.HasValue == false)
                    return null;
                //查出按讚列表
                var checkLikeArray = (commentOuterKey != null && commentOuterKey != string.Empty) ? GetLikeArrayByOuterKey(commentOuterKey) : GetLikeArrayByEventId(eventId.Value);
                result.ActivityOuterKey = activityOuterKey;

                //要按讚
                if (checkLikeArray.FirstOrDefault(t => t == memberInfo.Account) == null)
                {
                    var proxy = DoLike(memberInfo, eventId.Value, commentOuterKey);
                    if (proxy != null)
                    {
                        result.MessageEnum = proxy.MessageEnum;
                        result.CommentOuterKey = proxy.CommentOuterKey;
                        result.ReplyOuterKey = proxy.ReplyOuterKey;
                        result.IsLike = true;
                        //加按讚者
                        checkLikeArray.Add(memberInfo.Account);
                    }

                }
                //取消讚
                else
                {
                    var proxy = CancelLike(memberInfo, eventId.Value, commentOuterKey);
                    if (proxy != null)
                    {
                        var arrayProxy = checkLikeArray.ToList();
                        arrayProxy.Remove(memberInfo.Account.ToLower());
                        //去掉按讚者
                        checkLikeArray = arrayProxy;
                        result.MessageEnum = proxy.MessageEnum;
                        result.CommentOuterKey = proxy.CommentOuterKey;
                        result.ReplyOuterKey = proxy.ReplyOuterKey;
                        result.IsLike = false;
                    }

                }
                db.SaveChanges();
                result.LikeArray = checkLikeArray.ToArray();
                return result;
            }
        }
        /// <summary>
        /// 新增一個讚
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="outerKey"></param>
        /// <param name="isMsg"></param>
        /// <returns></returns>
        public bool Add(int memberId, Guid outerKey, bool? isMsg)
        {
            var db = _uow.DbContext;
            var dt = DateTime.UtcNow;
            var _type = false;
            if (isMsg == true)
                _type = true;
            
            var checkLike = db.LikeLog.Where(t => t.OuterKey == outerKey && t.MemberId == memberId);
            //有按讚，就刪除
            if (checkLike.Any())
            {
                var boolLike = checkLike.FirstOrDefault();
                db.LikeLog.Remove(boolLike);
            }
            else
            {
                #region // 1. 建立點讚活動物件                        
                var objLikeLog = new LikeLog()
                {
                    MemberId = memberId,
                    CreateUser = memberId,
                    Created = TimeData.Create(dt),
                    Updated = TimeData.Create(null),
                    Deleted = TimeData.Create(null),
                    IsMsg = _type,
                    OuterKey = outerKey
                };
                #endregion



                // 3. 寫入DB
                db.LikeLog.Add(objLikeLog);

                #region // 回傳給APP的物件

            }
            db.SaveChanges();
            #endregion
            return true;

        }

        /// <summary>
        /// 回傳主題討論或留言的結果
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="eventId"></param>
        /// <param name="commentOuterKey"></param>
        /// <returns></returns>
        private DiscussionUpdateLikeInfo DoLike(Member memberInfo, Guid eventId, string commentOuterKey)
        {
            var result = new DiscussionUpdateLikeInfo();
            //針對留言按讚
            if (commentOuterKey != null && commentOuterKey != string.Empty)
            {
                var db = _uow.DbContext;
                var commentEventId = Utility.OuterKeyHelper.CheckOuterKey(commentOuterKey);
                if (commentEventId.HasValue == false)
                    return null;
                //處理按讚資訊
                LikeDbProxy(memberInfo, commentEventId.Value, true);
                var commentInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == commentEventId.Value);
                //判斷是否為回覆[ true 是回覆 | false 是留言]
                var checkReply = (commentInfo.Parent != null && commentInfo.Parent != 0) ? true : false;
                result.MessageEnum = checkReply ? 3 : 2;
                result.CommentOuterKey = checkReply ?
                    Utility.OuterKeyHelper.GuidToPageToken(db.ActModuleMessage.FirstOrDefault(t => t.Id == commentInfo.Parent).OuterKey) :
                    Utility.OuterKeyHelper.GuidToPageToken(commentEventId.Value);
                result.ReplyOuterKey = checkReply ? commentOuterKey.ToString() : null;
            }
            //針對主題討論按讚
            else
            {
                LikeDbProxy(memberInfo, eventId, false);
                result.MessageEnum = 1;
            }
            return result;
        }

        /// <summary>
        /// 回傳主題討論或留言的結果
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="eventId"></param>
        /// <param name="commentOuterKey"></param>
        /// <returns></returns>
        private DiscussionUpdateLikeInfo CancelLike(Member memberInfo, Guid eventId, string commentOuterKey)
        {
            var result = new DiscussionUpdateLikeInfo();
            //針對留言按讚
            if (commentOuterKey != null && commentOuterKey != string.Empty)
            {
                var db = _uow.DbContext;
                var commentEventId = Utility.OuterKeyHelper.CheckOuterKey(commentOuterKey);
                if (commentEventId.HasValue == false)
                    return null;
                //處理按讚資訊
                UnLikeDbProxy(memberInfo, commentEventId.Value);
                var commentInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == commentEventId.Value);
                //判斷是否為回覆[ true 是回覆 | false 是留言]
                var checkReply = (commentInfo.Parent != null && commentInfo.Parent != 0) ? true : false;
                result.MessageEnum = checkReply ? 3 : 2;
                result.CommentOuterKey = checkReply ?
                     Utility.OuterKeyHelper.GuidToPageToken(db.ActModuleMessage.FirstOrDefault(t => t.Id == commentInfo.Parent).OuterKey) :
                     Utility.OuterKeyHelper.GuidToPageToken(commentEventId.Value);
                result.ReplyOuterKey = checkReply ? commentOuterKey.ToString() : null;
            }
            //針對主題討論按讚
            else
            {
                UnLikeDbProxy(memberInfo, eventId);
                result.MessageEnum = 1;
            }
            return result;
        }


        /// <summary>
        /// 處理按讚資料
        /// </summary>
        /// <param name="memberInfo">按讚者資訊</param>
        /// <param name="likeOuterKey">主題討論代碼/留言代碼</param>
        /// <param name="IsMsg">是否為留言</param>
        /// <returns></returns>
        private LikeLog LikeDbProxy(Member memberInfo, Guid likeOuterKey, bool IsMsg)
        {
            var db = _uow.DbContext;
            var result = new Infrastructure.Entity.LikeLog();
            //判斷是否曾經按過讚
            result = db.LikeLog.FirstOrDefault(t => t.OuterKey == likeOuterKey && t.MemberId == memberInfo.Id);
            if (result != null)
            {
                result.Deleted = TimeData.Create(null);
                result.DeleteUser = null;
            }
            //新增按讚資訊
            else
            {
                result = new Infrastructure.Entity.LikeLog()
                {
                    Created = TimeData.Create(DateTime.UtcNow),
                    CreateUser = memberInfo.Id,
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    MemberId = memberInfo.Id,
                    IsMsg = IsMsg,
                    OuterKey = likeOuterKey
                };
                db.LikeLog.Add(result);
            }
            return result;
        }

        /// <summary>
        /// 處理取消讚資料
        /// </summary>
        /// <param name="memberInfo">按讚者資訊</param>
        /// <param name="likeOuterKey">主題討論代碼/留言代碼</param>
        /// <returns></returns>
        private LikeLog UnLikeDbProxy(Member memberInfo, Guid likeOuterKey)
        {
            var db = _uow.DbContext;
            var result = new Infrastructure.Entity.LikeLog();
            //判斷是否曾經按過讚
            result = db.LikeLog.FirstOrDefault(t => t.OuterKey == likeOuterKey && t.MemberId == memberInfo.Id);
            if (result != null)
            {
                result.Deleted = TimeData.Create(DateTime.UtcNow);
                result.DeleteUser = memberInfo.Id;
            }
            return result;
        }
    }
}

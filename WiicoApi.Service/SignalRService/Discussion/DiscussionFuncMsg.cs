using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SignalRService.Discussion
{
    public class DiscussionFuncMsg
    {
        private readonly GenericUnitOfWork _uow = new GenericUnitOfWork();
        private DiscussionFuncLike discussionLikeService = null;

        /// <summary>
        /// 查詢回覆留言列表 - 用在推播與新增通知
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.BusinessObject.MemberCacheData> GetReplyMemberList(Guid outerKey, Member replyMember)
        {
            var db = _uow.DbContext;
            var actInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == outerKey);
            var replys = new List<IGrouping<int?, Infrastructure.Entity.ActModuleMessage>>();
            var replyMembers = new List<Infrastructure.BusinessObject.MemberCacheData>();
            //查主題討論的留言列表
            if (actInfo != null)
            {
                replys = db.ActModuleMessage.Where(t => t.ActivityId == actInfo.Id && t.Parent == null).GroupBy(t => t.CreateUser).ToList();
                replyMembers = GetReplyMembers(replys);
                //要包含主題討論建立者
                var creatorInfo = db.Members.Find(actInfo.CreateUser.Value);
                if (replyMembers.FirstOrDefault(t => t.Account == creatorInfo.Account && t.Id == creatorInfo.Id) == null)
                    replyMembers.Add(new Infrastructure.BusinessObject.MemberCacheData() { Account = creatorInfo.Account, Id = creatorInfo.Id, ConnectionId = creatorInfo.ConnectionId });
            }
            else
            {
                var msgInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == outerKey);
                if (msgInfo != null)
                {
                    actInfo = db.Activitys.Find(msgInfo.ActivityId);
                    var creatorInfo = db.Members.Find(actInfo.CreateUser.Value);
                    //查留言的回覆列表
                    if (msgInfo.Parent.HasValue)
                    {
                        var parentMsg = db.ActModuleMessage.Find(msgInfo.Parent);
                        var parentMemberInfo = db.Members.Find(parentMsg.CreateUser.Value);
                        replys = db.ActModuleMessage.Where(t => t.Parent == parentMsg.Id).GroupBy(t => t.CreateUser).ToList();
                        replyMembers = GetReplyMembers(replys);
                        if (replyMembers.FirstOrDefault(t => t.Account == parentMemberInfo.Account && t.Id == parentMemberInfo.Id) == null)
                            replyMembers.Add(new Infrastructure.BusinessObject.MemberCacheData() { Account = parentMemberInfo.Account, Id = parentMemberInfo.Id, ConnectionId = parentMemberInfo.ConnectionId });

                    }//查主題討論的留言列表
                    else if (msgInfo.Parent == null)
                    {
                        replys = db.ActModuleMessage.Where(t => t.ActivityId == msgInfo.ActivityId && t.Parent == null).GroupBy(t => t.CreateUser).ToList();
                        replyMembers = GetReplyMembers(replys);
                    }
                    if (replyMembers.FirstOrDefault(t => t.Account == creatorInfo.Account && t.Id == creatorInfo.Id) == null)
                        replyMembers.Add(new Infrastructure.BusinessObject.MemberCacheData() { Account = creatorInfo.Account, Id = creatorInfo.Id, ConnectionId = creatorInfo.ConnectionId });
                }
            }
            //去掉自己
            replyMembers.RemoveAll(t => t.Account == replyMember.Account && t.Id == replyMember.Id);
            return replyMembers;
        }

        /// <summary>
        /// 取得留言內頁資訊
        /// </summary>
        /// <param name="token">使用者代碼</param>
        /// <param name="outerKey">留言/回覆代碼</param>
        /// <param name="maxResult">顯示回覆列表筆數 - 預設10筆</param>
        /// <returns></returns>
        public DiscussionCommentDetail GetCommentDetail(Guid token, string outerKey, int? maxResult = 10)
        {
            var db = _uow.DbContext;
            var result = new DiscussionCommentDetail();
            result.Replys = new List<DiscussionMessage>();
            discussionLikeService = new DiscussionFuncLike();
            var msgEventId = Service.Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            //用於取得msgEventId的留言資訊
            var msgInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == msgEventId);
            //如果是回覆則取得該回覆的留言資訊 | 若是留言則直接存取留言資訊
            var commentInfo = (db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == msgEventId && t.Parent != null)) != null ? db.ActModuleMessage.Find(db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == msgEventId).Parent) : msgInfo;
            if (commentInfo != null)
            {

                var commentCreatorInfo = new MemberService().UserIdToAccount(commentInfo.CreateUser.Value);
                //  var commentCreatorInfo = new Service.MemberService().GetPhotoMember(db.Members.Find(commentInfo.CreateUser).Account);
                result.Comment = new DiscussionMessage()
                {
                    CreateTime = commentInfo.Created.Utc.Value.ToLocalTime(),
                    CreatorAccount = commentCreatorInfo.Account,
                    CreatorName = commentCreatorInfo.Name,
                    CreatorPhoto = commentCreatorInfo.Photo,
                    Message = commentInfo.Content,
                    EventId = commentInfo.OuterKey,
                    Parent = commentInfo.Parent
                };

                var commentPhoto = GetDiscussionMessageFileByMessageId(commentInfo.Id);
                result.Comment.Photos = commentPhoto != null ? commentPhoto : new List<FileStorageViewModel>();

                var commentLikeArray = discussionLikeService.GetLikeArrayByEventId(commentInfo.OuterKey);
                result.Comment.LikeArray = commentLikeArray != null ? commentLikeArray.ToArray() : null;
                var replys = db.ActModuleMessage.Where(t => t.Parent == commentInfo.Id).OrderByDescending(t => t.Created.Utc).ToList();
                result.Comment.ReplyCount = replys.FirstOrDefault() != null ? replys.Count() : 0;
                result.CommentCount = replys.FirstOrDefault() != null ? replys.Count() : 0;



                //判斷現在是否要查留言內頁的資訊
                if (msgInfo.Parent.HasValue)
                {
                    //篩選該msg以後的留言[包含]
                    replys = replys.Where(t => t.Id >= msgInfo.Id).ToList();
                    replys.Reverse();
                    result.OlderCount = result.CommentCount - replys.Count();
                    replys = replys.Take(maxResult.Value).ToList();
                }
                else
                {
                    result.OlderCount = replys.Count() > maxResult.Value ? replys.Count() - maxResult.Value : 0;
                    replys = replys.Take(maxResult.Value).ToList();
                    replys.Reverse();
                }

                foreach (var reply in replys)
                {
                    var replyCreatorInfo = new MemberService().UserIdToAccount(reply.CreateUser.Value);
                    //   var replyCreatorInfo = new Service.MemberService().GetPhotoMember(db.Members.Find(reply.CreateUser).Account);
                    var tempReply = new DiscussionMessage()
                    {
                        CreateTime = reply.Created.Utc.Value.ToLocalTime(),
                        CreatorAccount = replyCreatorInfo.Account,
                        CreatorName = replyCreatorInfo.Name,
                        CreatorPhoto = replyCreatorInfo.Photo,
                        Message = reply.Content,
                        EventId = reply.OuterKey,
                        Parent = reply.Parent
                    };
                    var replyPhoto = GetDiscussionMessageFileByMessageId(reply.Id);
                    tempReply.Photos = replyPhoto != null ? replyPhoto : new List<FileStorageViewModel>();

                    var likeArray = discussionLikeService.GetLikeArrayByEventId(reply.OuterKey);
                    tempReply.LikeArray = likeArray != null ? likeArray.ToArray() : null;

                    if (reply.TagActModuleMessageId != null && reply.TagActModuleMessageId != 0)
                    {
                        var replyInfo = db.ActModuleMessage.Find(reply.TagActModuleMessageId);
                        if (replyInfo != null)
                        {
                            var replyMemberInfo = db.Members.Find(replyInfo.CreateUser);
                            tempReply.ReplyOuterKey = reply.OuterKey.ToString();
                            tempReply.ReplyName = replyMemberInfo.Name;
                        }
                    }
                    tempReply.ReplyCount = db.ActModuleMessage.FirstOrDefault(t => t.Parent == reply.Id) != null ? db.ActModuleMessage.Where(t => t.Parent == reply.Id).Count() : 0;
                    result.Replys.Add(tempReply);
                }
            }
            return result;
        }


        /// <summary>
        /// 取得留言&回覆列表
        /// </summary>
        /// <param name="eventId">主題討論活動代碼</param>
        /// <param name="msgEventId">留言代碼 / 回覆留言代碼</param>
        /// <param name="isLoadNewer">是否往新的查</param>
        /// <returns></returns>
        public List<DiscussionMessage> GetMessageList(Guid eventId, Guid? msgEventId = null, bool? isLoadNewer = true)
        {
            var db = _uow.DbContext;
            var result = new List<DiscussionMessage>();
            //預設查詢主題討論的留言列表
            var sqlData = from amm in db.ActModuleMessage
                          join a in db.Activitys on amm.ActivityId equals a.Id
                          where a.OuterKey == eventId
                          orderby amm.Created.Utc
                          select amm;

            if (sqlData.FirstOrDefault() != null)
            {
                discussionLikeService = new DiscussionFuncLike();
                //判斷是否有msg查詢條件
                if (msgEventId != null)
                {
                    //查出該msg的資訊
                    var setMsgInfo = sqlData.FirstOrDefault(t => t.OuterKey == msgEventId.Value);
                    if (setMsgInfo != null)
                    {
                        //判斷現在是否要查留言內頁的資訊
                        if (setMsgInfo.Parent.HasValue)
                            sqlData = db.ActModuleMessage.Where(t => t.Parent == setMsgInfo.Parent);
                        else
                            sqlData = sqlData.Where(t => t.Parent == null || t.Parent == 0);

                        //查新的
                        if (isLoadNewer.Value)
                            //篩選該msg以後的留言[包含]
                            sqlData = sqlData.Where(t => t.Id >= setMsgInfo.Id);
                        //查舊的
                        else
                            sqlData = sqlData.Where(t => t.Id < setMsgInfo.Id);
                    }
                }
                else
                    sqlData = sqlData.Where(t => t.Parent == null || t.Parent == 0);
                //塞資料
                foreach (var msg in sqlData)
                {
                    //取得留言者資訊
                    var memberInfo = new MemberService().UserIdToAccount(msg.CreateUser.Value);
                    //var memberInfo = new iThink.Service.Service.MemberService().GetPhotoMember(db.Members.Find(msg.CreateUser.Value).Account);


                    //取得留言檔案資訊
                    var msgFile = GetDiscussionMessageFileByMessageId(msg.Id);
                    var tempMsg = new DiscussionMessage();
                    tempMsg.Id = msg.Id;
                    tempMsg.CreateTime = msg.Created.Local.Value;
                    var likeArrays = discussionLikeService.GetLikeArrayByEventId(msg.OuterKey);
                    tempMsg.LikeArray = likeArrays != null ? likeArrays.ToArray() : new string[0];
                    if (msgFile != null)
                        tempMsg.Photos = msgFile;
                    else
                        tempMsg.Photos = new List<FileStorageViewModel>();
                    tempMsg.Message = msg.Content;
                    tempMsg.EventId = msg.OuterKey;
                    tempMsg.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(msg.OuterKey);
                    tempMsg.Parent = msg.Parent;
                    if (memberInfo != null)
                    {
                        tempMsg.CreatorAccount = memberInfo.Account;
                        tempMsg.CreatorName = memberInfo.Name;
                        tempMsg.CreatorPhoto = memberInfo.Photo;
                    }
                    //是否有tag某篇留言
                    if (msg.TagActModuleMessageId != null && msg.TagActModuleMessageId != 0)
                    {
                        var replyInfo = db.ActModuleMessage.Find(msg.TagActModuleMessageId);
                        if (replyInfo != null)
                        {
                            var replyMemberInfo = db.Members.Find(replyInfo.CreateUser);
                            tempMsg.ReplyOuterKey = replyInfo.OuterKey.ToString();
                            tempMsg.ReplyName = replyMemberInfo.Name;
                        }
                    }
                    //查詢回覆數量
                    tempMsg.ReplyCount = db.ActModuleMessage.FirstOrDefault(t => t.Parent == msg.Id) != null ? db.ActModuleMessage.Where(t => t.Parent == msg.Id).Count() : 0;
                    result.Add(tempMsg);
                }
            }

            return result;
        }

        /// <summary>
        /// 取出該主題討論的所有留言與回覆
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public IQueryable<ActModuleMessage> GetAllMsgList(Guid outerKey)
        {
            var db = _uow.DbContext;
            var result = from amm in db.ActModuleMessage
                         join a in db.Activitys on amm.ActivityId equals a.Id
                         where a.OuterKey == outerKey
                         select amm;
            return result;
        }

        /// <summary>
        /// 取得主題討論底下的留言列表
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msgEventId"></param>
        /// <param name="isLoadNewer"></param>
        /// <returns></returns>
        public List<DiscussionMessage> GetDiscussionMsgList(Guid eventId, Guid msgEventId, bool? isLoadNewer = true)
        {
            var result = new List<DiscussionMessage>();
            var db = _uow.DbContext;
            //預設查詢主題討論的留言列表
            var sqlData = from amm in db.ActModuleMessage
                          join a in db.Activitys on amm.ActivityId equals a.Id
                          where a.OuterKey == eventId
                          orderby amm.Created.Utc
                          select amm;

            if (sqlData.FirstOrDefault() != null)
            {
                discussionLikeService = new DiscussionFuncLike();
                //查出該msg的資訊
                var setMsgInfo = (sqlData.FirstOrDefault(t => t.OuterKey == msgEventId && t.Parent != null)) != null ? sqlData.FirstOrDefault(t => t.Id == (sqlData.FirstOrDefault(tt => tt.OuterKey == msgEventId)).Parent) : sqlData.FirstOrDefault(t => t.OuterKey == msgEventId);
                if (setMsgInfo != null)
                {
                    sqlData = sqlData.Where(t => t.Parent == null || t.Parent == 0);
                    //查新的
                    if (isLoadNewer.Value)
                        //篩選該msg以後的留言[包含]
                        sqlData = sqlData.Where(t => t.Id >= setMsgInfo.Id);
                    //查舊的
                    else
                        sqlData = sqlData.Where(t => t.Id < setMsgInfo.Id);
                }
                //塞資料
                foreach (var msg in sqlData)
                {
                    //取得留言者資訊
                    var memberInfo = new MemberService().UserIdToAccount(msg.CreateUser.Value);
                    // var memberInfo = new iThink.Service.Service.MemberService().GetPhotoMember(db.Members.Find(msg.CreateUser.Value).Account);
                    //取得留言檔案資訊
                    var msgFile = GetDiscussionMessageFileByMessageId(msg.Id);
                    var tempMsg = new DiscussionMessage();
                    tempMsg.Id = msg.Id;
                    tempMsg.CreateTime = msg.Created.Local.Value;
                    tempMsg.LikeArray = discussionLikeService.GetLikeArrayByEventId(msg.OuterKey).ToArray();
                    if (msgFile != null)
                        tempMsg.Photos = msgFile;
                    else
                        tempMsg.Photos = new List<FileStorageViewModel>();
                    tempMsg.Message = msg.Content;
                    tempMsg.EventId = msg.OuterKey;
                    tempMsg.Parent = msg.Parent;
                    if (memberInfo != null)
                    {
                        tempMsg.CreatorAccount = memberInfo.Account;
                        tempMsg.CreatorName = memberInfo.Name;
                        tempMsg.CreatorPhoto = memberInfo.Photo;
                    }
                    //是否有tag某篇留言
                    if (msg.TagActModuleMessageId != null && msg.TagActModuleMessageId != 0)
                    {
                        var replyInfo = db.ActModuleMessage.Find(msg.TagActModuleMessageId);
                        if (replyInfo != null)
                        {
                            var replyMemberInfo = db.Members.Find(replyInfo.CreateUser);
                            tempMsg.ReplyOuterKey = replyInfo.OuterKey.ToString();
                            tempMsg.ReplyName = replyMemberInfo.Name;
                        }
                    }
                    //查詢回覆數量
                    tempMsg.ReplyCount = db.ActModuleMessage.FirstOrDefault(t => t.Parent == msg.Id) != null ? db.ActModuleMessage.Where(t => t.Parent == msg.Id).Count() : 0;
                    result.Add(tempMsg);
                }
            }
            return result;
        }

        /// <summary>
        /// 取得主題討論留言的檔案
        /// </summary>
        /// <param name="messageId">主題討論留言編號</param>
        /// <returns></returns>
        public List<FileStorageViewModel> GetDiscussionMessageFileByMessageId(int messageId)
        {
            var db = _uow.DbContext;
            var fileService = new FileService();
            var sqlData = (from f in db.FileStorage
                           join df in db.DiscussionFile on f.Id equals df.FileId
                           where df.MessageId == messageId
                           select f).ToList();

            if (sqlData.Count() > 0)
                return fileService.ImageFileProcess(sqlData);
            else
                return null;
        }

        /// <summary>
        /// 新增留言
        /// </summary>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="msg">留言內容</param>
        /// <param name="memberId">留言者編號</param>
        /// <param name="activityOuterKey">主題討論活動代碼</param>
        /// <param name="commentOuterKey">回覆留言代碼</param>
        /// <param name="replyOuterKey">tag某個回覆</param>
        /// <returns></returns>
        public DiscussionSendMsg AddMessage(string circleKey, string msg, int memberId, string activityOuterKey, string commentOuterKey, string replyOuterKey)
        {
            var result = new DiscussionSendMsg();
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(activityOuterKey);
            if (eventId.HasValue == false)
                return null;
            var db = _uow.DbContext;

            var activityInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId.Value);
            var discussionInfo = db.ActDiscussion.FirstOrDefault(t => t.EventId == eventId.Value);
            if (activityInfo != null && discussionInfo != null)
            {
                int? replayId = null;
                var isComment = true;
                var msgOuterKey = Guid.NewGuid();
                var creatorInfo = new MemberService().UserIdToAccount(memberId);
                //  var creatorInfo = new Service.MemberService().GetPhotoMember(db.Members.Find(memberId).Account);

                var actModuleMessageEntity = new ActModuleMessage()
                {
                    ActivityId = activityInfo.Id,
                    MsgType = "text",
                    Content = msg,
                    ModuleType = "discussion",
                    OuterKey = msgOuterKey,
                    CreateUser = memberId,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Updated = TimeData.Create(null),
                    Deleted = TimeData.Create(null),
                    Visibility = true,

                };

                //新增留言的回覆
                if (commentOuterKey != null && commentOuterKey != string.Empty)
                {
                    var commentMessageEventId = Utility.OuterKeyHelper.CheckOuterKey(commentOuterKey);
                    //留言資訊
                    var commentInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == commentMessageEventId);
                    if (commentInfo != null)
                    {
                        var replyCount = db.ActModuleMessage.Where(t => t.Parent == commentInfo.Id).Count();
                        result.CommentOuterkey = commentOuterKey;
                        result.CommentCount = replyCount + 1;
                        actModuleMessageEntity.Parent = commentInfo.Id;
                        replayId = commentInfo.Id;
                        isComment = false;
                    }
                }
                result.Comment = new DiscussionMessage()
                {
                    EventId = msgOuterKey,
                    CreateTime = DateTime.Now,
                    CreatorAccount = creatorInfo.Account,
                    CreatorName = creatorInfo.Name,
                    CreatorPhoto = creatorInfo.Photo,
                    Message = msg,
                    Parent = replayId,
                    ReplyCount = 0,
                    LikeArray = new string[0]
                };

                if (replyOuterKey != null && replyOuterKey != string.Empty)
                {
                    var replyDBOuterKey = Utility.OuterKeyHelper.CheckOuterKey(replyOuterKey);
                    var replyInfo = db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == replyDBOuterKey);
                    if (replyInfo != null)
                    {
                        var replyMemberInfo = db.Members.Find(replyInfo.CreateUser);
                        result.Comment.ReplyOuterKey = replyOuterKey;
                        result.Comment.ReplyName = replyMemberInfo.Name;
                        actModuleMessageEntity.TagActModuleMessageId = replyInfo.Id;
                    }
                }
                db.ActModuleMessage.Add(actModuleMessageEntity);

                //這裡儲存是為了ActDisscussionMsg的ActModuleMsgId
                db.SaveChanges();

                var discussionMsgEntity = new ActDiscussionMsg()
                {
                    ActModuleMsgId = actModuleMessageEntity.Id,
                    ActDiscussionId = discussionInfo.Id,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Updated = TimeData.Create(null),
                    Deleted = TimeData.Create(null),
                    CreateUser = memberId,
                    OuterKey = msgOuterKey
                };
                db.ActDiscussionMsg.Add(discussionMsgEntity);
                db.SaveChanges();

                result.ActivityOuterKey = activityOuterKey;
                result.IsComment = isComment;
                result.Id = actModuleMessageEntity.Id;
                result.TotalCount = GetAllMsgList(eventId.Value).Count();
                result.CircleKey = circleKey;
            }
            else
                result = null;
            return result;
        }

        /// <summary>
        /// 暫存回覆者清單
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns></returns>
        private List<Infrastructure.BusinessObject.MemberCacheData> GetReplyMembers(List<IGrouping<int?, Infrastructure.Entity.ActModuleMessage>> msgs)
        {
            var db = _uow.DbContext;
            var result = new List<Infrastructure.BusinessObject.MemberCacheData>();
            if (msgs.FirstOrDefault() != null)
            {
                foreach (var replyMember in msgs)
                {

                    var member = replyMember.Key.HasValue ? db.Members.Find(replyMember.Key.Value) : null;
                    if (member != null)
                        result.Add(new Infrastructure.BusinessObject.MemberCacheData() { Account = member.Account, Id = member.Id, ConnectionId = member.ConnectionId });
                }
            }
            return result;
        }
    }
}

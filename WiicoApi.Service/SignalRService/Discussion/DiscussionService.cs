using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using static WiicoApi.Repository.QueryCondition;

namespace WiicoApi.Service.SignalRService.Discussion
{
    public class DiscussionService
    {
        private readonly GenericUnitOfWork _uow;
        private readonly LikeService likeService = new LikeService();
        private readonly CacheService cacheService = new CacheService();
        private readonly MessageService messageService = new MessageService();
       // private readonly GoogleDriveService googleDriveService = new GoogleDriveService();
        private readonly MemberService memberService = new MemberService();
        private readonly PushService pushService = new PushService();
        private readonly ActivityService activityService = new ActivityService();
        private readonly LearningCircleService learningCircleService = new LearningCircleService();

        private readonly FileService fileService = new FileService();
        private readonly DiscussionFuncMsg discussionMsgService = new DiscussionFuncMsg();
        private readonly DiscussionFuncLike discussionLikeService = new DiscussionFuncLike();
        private readonly DiscussionFuncFile discussionFileService = new DiscussionFuncFile();


        public DiscussionService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得個人主題討論紀錄
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.DiscussionViewModel GetForMem(Guid eventId, int memberId)
        {
            //取得主題討論資訊
            var db = _uow.DbContext;
            var adr = _uow.EntityRepository<ActDiscussion>().GetFirst(x => x.EventId == eventId);
            if (adr != null)
            {
                //取得留言資訊
                //var msg = _msgRep.Get(x => x.ActDiscussionId == adr.Id && x.CreateUser == memberId);
                var msg = db.ActDiscussionMsg.Where(x => x.ActDiscussionId == adr.Id);


                //取得活動牆資訊
                var ar = db.Activitys.Where(x => x.OuterKey == eventId).FirstOrDefault();
                if (ar.CardisShow == true)
                {
                    //取得建立主題討論者資訊
                    var memberInfo = _uow.MembersRepo.MemberInfo(adr.CreateUser.Value);

                    //取得發言(登入)者的資訊
                    var me = _uow.MembersRepo.MemberInfo(memberId);

                    //var fileInfo = googleService.GetFileList(systemManager,);
                    var vm = new Infrastructure.ViewModel.DiscussionViewModel()
                    {
                        ActivityId = adr.Id,
                        LearningId = adr.LearningId,
                        EventId = eventId,
                        OuterKey = eventId,
                        GoogleDriveFolder = adr.GoogleDriveUrl,
                        Creator = adr.CreateUser.Value.ToString(),
                        CreateDate = Convert.ToDateTime(adr.Created.Local.ToString()),
                        Created_Utc = Convert.ToDateTime(adr.Created.Utc.Value),
                        Deleted_Utc = adr.Deleted.Utc,
                        ModuleKey = Utility.ParaCondition.ModuleType.Discussion,
                        CreatorAccount = memberInfo.Account,
                        CreatorName = memberInfo.Name,
                        Duration = ar.Duration,
                        Name = adr.Name,
                        CreatorPhoto = memberInfo.Photo,
                        MsgSenderPhoto = me.Photo,
                        Description = adr.Description,
                        ToRoomId = ar.ToRoomId,
                        FileCount = adr.FileCount,
                        Publish_Utc = ar.Publish_Utc
                    };

                    vm.Msg = new List<Infrastructure.BusinessObject.MsgContent>();
                    vm.Like = new Infrastructure.BusinessObject.LikeCount();
                    //先查看本主題討論是否有點過讚
                    var discussionLike = likeService.CheckLikeInfo(eventId, adr.Name, false, memberId);
                    if (discussionLike != null)
                        vm.Like = discussionLike;

                    #region //取得該主題討論區的所有留言
                    if (msg != null)
                    {
                        foreach (var _msg in msg)
                        {
                            var resMsg = new Infrastructure.BusinessObject.MsgContent();
                            resMsg.Like = new Infrastructure.BusinessObject.LikeCount();
                            var _info = db.ActModuleMessage.Where(t => t.ActivityId == ar.Id && t.Id == _msg.ActModuleMsgId && t.Visibility != false).FirstOrDefault();

                            //如果沒被刪除或有資料時
                            if (_info != null)
                            {
                                // var _msgMember = 0;
                                resMsg.MsgInfo = _info;
                                //取得建立主題討論者資訊
                                var msgMember = _uow.MembersRepo.MemberInfo(_info.CreateUser.Value);
                                resMsg.MemberInfo = new Infrastructure.ValueObject.LearningCircleMemberInfo()
                                {
                                    AccountId = msgMember.Account,
                                    MemberId = msgMember.Id,
                                    Email = msgMember.Email,
                                    MemberName = msgMember.Name,
                                    Picture = msgMember.Photo
                                };
                                /* if (_info.CreateUser != null)
                                 {
                                     _msgMember = Convert.ToInt32(_info.CreateUser);
                                 }*/
                                //查看是否有該留言的讚
                                var _likeInfo = likeService.CheckLikeInfo(_info.OuterKey, _info.Content, true, memberId);

                                //塞讚資訊
                                if (_likeInfo != null)
                                {
                                    resMsg.Like = _likeInfo;
                                    //vm.Like.Add(discussionLike);
                                }

                                //查看是否為自己留言
                                if (_info.CreateUser.Equals(memberId))
                                    resMsg.IsMy = true;
                                else
                                    resMsg.IsMy = false;

                                vm.Msg.Add(resMsg);
                            }

                        }
                    }
                    #endregion
                    return vm;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 新增一筆主題討論活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="name"></param>
        /// <param name="googleDriveFileId"></param>
        /// <param name="description"></param>
        /// <param name="fileCount"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        private Infrastructure.BusinessObject.DiscussionEvent Add(string circleKey, int memberId, string name, string googleDriveFileId, string description, int fileCount, int tagId)
        {
            var db = _uow.DbContext;
            Guid eveId = Guid.NewGuid();
            DateTime? dt = DateTime.UtcNow;
            int id = cacheService.GetCircle(circleKey).Id;

            #region // 1. 建立主題討論活動物件                        
            var objDiscussion = new ActDiscussion()
            {
                LearningId = id,
                Name = name,
                EventId = eveId,
                CreateUser = memberId,
                Created = Infrastructure.Property.TimeData.Create(dt),
                Updated = Infrastructure.Property.TimeData.Create(null),
                Deleted = Infrastructure.Property.TimeData.Create(null),
                Description = description,
                GoogleDriveUrl = googleDriveFileId,
                TagId = tagId,
                Visibility = true,
                Enable = true,
                FileCount = fileCount
            };
            #endregion
            #region // 2. 替主題討論活動建立一筆訊息物件
            Activitys objAct = new Activitys()
            {
                Duration = 0,
                ModuleKey = Utility.ParaCondition.ModuleType.Discussion,
                ToRoomId = circleKey,
                CreateUser = memberId,
                Created = Infrastructure.Property.TimeData.Create(dt),
                Updated = Infrastructure.Property.TimeData.Create(null),
                Deleted = Infrastructure.Property.TimeData.Create(null),
                OuterKey = eveId,
                StartDate = dt,
                IsActivity = true,
                CardisShow = true,
                Publish_Utc = dt
            };
            #endregion

            // 3. 寫入DB
            db.ActDiscussion.Add(objDiscussion);
            db.Activitys.Add(objAct);

            //要先儲存Activity的內容才能生出ActivityId
            db.SaveChanges();


            #region // 回傳給APP的物件
            var data = new Infrastructure.BusinessObject.DiscussionEvent()
            {
                ClassId = circleKey,
                OuterKey = eveId,
                Creator = memberId.ToString(),
                CreateDate = Convert.ToDateTime(dt)

            };
            #endregion

            return data;
        }

        /// <summary>
        /// 在水道頁長出新增活動之後的主題討論活動
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        ///  <param name="circleKey"></param>
        /// <returns></returns>
        private Infrastructure.ViewModel.ActivitysViewModel addPanelInfo(Guid eventId, int memberId, string circleKey)
        {
            var db = _uow.DbContext;
            var data = new Infrastructure.ViewModel.ActivitysViewModel();
            var activityInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (activityInfo != null)
            {

                DateTime dt = DateTime.UtcNow;
                // 回傳給APP的物件
                var me = db.Members.SingleOrDefault(x => x.Id == memberId);
                var photoInfo = me.Photo;
                if (me != null)
                {
                    data = new Infrastructure.ViewModel.ActivitysViewModel()
                    {
                        Id = 0,
                        CreatorAccount = me.Account,
                        CreatorName = me.Name,
                        CreatorPhoto = photoInfo,
                        ToRoomId = circleKey.ToLower(),
                        ModuleKey = "discussion",
                        Created_Utc = dt,
                        ReadMark = false,
                        PositionMark = false,
                        OuterKey = eventId,
                        Publish_Utc = activityInfo.Publish_Utc
                    };
                }
            }
            return data;
        }
        

        /// <summary>
        /// 長出主題討論活動列表資料
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<DiscussionModuleList> GetDiscussionList(string circleKey, int? pages = 1, int? rows = 20)
        {
            var db = _uow.DbContext;
            var acts = activityService.GetActivityListByModuleKey(circleKey, "discussion", pages, rows).ToList();

            if (acts.FirstOrDefault() == null)
                return null;
            var resultList = new List<DiscussionModuleList>();

            foreach (var act in acts)
            {
                var checkComment = db.ActModuleMessage.Where(t => t.ActivityId == act.Id);
                var totalCount = checkComment.FirstOrDefault() != null ? checkComment.Count() : 0;
                var discussionInfo = GetDBDiscussionInfo(act.OuterKey.ToString());

                if (discussionInfo == null)
                    continue;
                var checkFile = (from df in db.DiscussionFile
                                 join fs in db.FileStorage on df.FileId equals fs.Id
                                 where df.DiscussionId == discussionInfo.Id && df.MessageId == null
                                 select df).ToList();

                var fileCount = checkFile.FirstOrDefault() != null ? checkFile.Count() : 0;
                var likeArray = discussionLikeService.GetLikeArrayByEventId(act.OuterKey);
                var resultData = new DiscussionModuleList()
                {
                    TotalCount = totalCount,
                    CreateTime = discussionInfo.Created != null ? discussionInfo.Created.Local.Value : DateTime.Now,
                    FileCount = fileCount,
                    LikeArray = new List<string>(),
                    ModuleKey = "discussion",
                    EventId = act.OuterKey,
                    OuterKey = Utility.OuterKeyHelper.GuidToPageToken(act.OuterKey),
                    Publish_date = act.Publish_Utc.HasValue ? act.Publish_Utc.Value.ToLocalTime() : DateTime.Now,
                    Title = discussionInfo.Name,
                    GroupId = circleKey
                };
                if (likeArray != null)
                    resultData.LikeArray = likeArray.ToList();
                resultList.Add(resultData);

            }
            return resultList;
        }

        /// <summary>
        /// 取得主題討論DB資訊
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public ActDiscussion GetDBDiscussionInfo(string outerKey)
        {
            var db = _uow.DbContext;
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);

            var result = eventId.HasValue ? db.ActDiscussion.FirstOrDefault(t => t.EventId == eventId.Value) : null;
            return result;
        }

        /// <summary>
        /// 取得主題討論詳細資訊
        /// </summary>
        /// <param name="outerKey">request來的代碼</param>
        /// <param name="maxResult">特殊查詢 預設為10</param>
        /// <returns></returns>
        public DiscussionDetail GetDetailByOuterKey(string outerKey, int? maxResult = 10)
        {
            var db = _uow.DbContext;
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (eventId.HasValue == false)
                return null;
            //取得活動資料
            var sqlData = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId.Value);

            if (sqlData != null)
            {
                return GetDetailByEventId(eventId.Value, null, maxResult);
            }

            else
            {
                var result = GetDetailByMessageEventId(eventId.Value, maxResult);
                if (result != null)
                {
                    var msgInfo = messageService.GetMsgDBInfoByEventId(eventId.Value);
                    if (msgInfo.Parent.HasValue)
                        result.ToCommentOuterKey = outerKey;
                }
                return result;
            }
        }

        /// <summary>
        /// 取得主題討論詳細資訊 - 顯示留言為該msgEventId之後的留言(包含該MsgEventId)
        /// </summary>
        /// <param name="msgEventId"></param>
        /// <returns></returns>
        public DiscussionDetail GetDetailByMessageEventId(Guid msgEventId, int? maxResult = 10)
        {
            var db = _uow.DbContext;
            var activityInfo = (from a in db.Activitys
                                join amm in db.ActModuleMessage on a.Id equals amm.ActivityId
                                where amm.OuterKey == msgEventId
                                select a).FirstOrDefault();
            if (activityInfo != null)
                return GetDetailByEventId(activityInfo.OuterKey, msgEventId, maxResult);
            else
                return null;
        }

        /// <summary>
        /// 編輯主題討論後的查詢
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionDetail GetUpdateDetailByEventId(Guid eventId)
        {
            var db = _uow.DbContext;

            //取得活動資料
            var sqlData = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (sqlData == null)
                return null;
            else
            {
                //取得主題討論詳細資料
                var discussionInformation = db.ActDiscussion.Where(t => t.EventId == eventId).FirstOrDefault();
                var creatorAccount = db.Members.Find(sqlData.CreateUser);
                var outerKey = Service.Utility.OuterKeyHelper.GuidToPageToken(eventId);
                var result = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionDetail()
                {
                    ModuleKey = "discussion",
                    sOuterKey = outerKey,
                    Publish_Utc = sqlData.Publish_Utc,
                    ToRoomId = sqlData.ToRoomId,
                    Id = sqlData.Id,
                    ActivityDate = sqlData.ActivityDate,
                    StartDate = sqlData.StartDate,
                    Duration = sqlData.Duration,
                    Deleted_Utc = sqlData.Deleted.Utc,
                    Title = discussionInformation.Name,
                    Content = discussionInformation.Description,
                    Created_Utc = discussionInformation.Created.Utc.Value
                };

                //查詢留言者資訊
                if (creatorAccount != null)
                {
                    // var creatorInfo = new iThink.Service.Service.MemberService().GetPhotoMember(creatorAccount);
                    result.CreatorAccount = creatorAccount.Account;
                    result.CreatorPhoto = creatorAccount.Photo;
                    result.CreatorName = creatorAccount.Name;
                }

                //查詢主題討論檔案
                var checkFiles = discussionFileService.GetDiscussionFileByEventId(eventId);
                if (checkFiles != null)
                    result.FileList = checkFiles;

                //查詢主題討論按讚資訊
                var checkLike = discussionLikeService.GetLikeArrayByEventId(eventId);
                if (checkLike != null)
                    result.LikeArray = checkLike.ToArray();

                //所有留言回覆列表
                var totalMsgList = discussionMsgService.GetAllMsgList(eventId);
                result.TotalCount = totalMsgList.Count();

                return result;
            }
        }

        /// <summary>
        /// 編輯主題討論後的查詢
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public DiscussionDetail GetUpdateDetailByEventId(Guid eventId, int discussionId, string[] removeFiles)
        {
            var db = _uow.DbContext;

            //刪除主題討論與檔案的關聯
            discussionFileService.DeleteDiscussionFileReference(discussionId, removeFiles);

            //取得活動資料
            var sqlData = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (sqlData == null)
                return null;
            else
            {
                //取得主題討論詳細資料
                var discussionInformation = db.ActDiscussion.Where(t => t.EventId == eventId).FirstOrDefault();
                var creatorAccount = db.Members.Find(sqlData.CreateUser);

                var outerKey = Utility.OuterKeyHelper.GuidToPageToken(eventId);
                var result = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionDetail()
                {
                    ModuleKey = "discussion",
                    sOuterKey = outerKey,
                    Publish_Utc = sqlData.Publish_Utc,
                    ToRoomId = sqlData.ToRoomId,
                    Id = sqlData.Id,
                    ActivityDate = sqlData.ActivityDate,
                    StartDate = sqlData.StartDate,
                    Duration = sqlData.Duration,
                    Deleted_Utc = sqlData.Deleted.Utc,
                    Title = discussionInformation.Name,
                    Content = discussionInformation.Description,
                    Created_Utc = discussionInformation.Created.Utc.Value
                };

                //查詢留言者資訊
                if (creatorAccount != null)
                {
                    // var creatorInfo = new iThink.Service.Service.MemberService().GetPhotoMember(creatorAccount);
                    result.CreatorAccount = creatorAccount.Account;
                    result.CreatorPhoto = creatorAccount.Photo;
                    result.CreatorName = creatorAccount.Name;
                }

                //查詢主題討論檔案
                var checkFiles = discussionFileService.GetDiscussionFileByEventId(eventId);
                if (checkFiles != null)
                    result.FileList = checkFiles;
                else
                    result.FileList = new List<FileStorageViewModel>();
                //查詢主題討論按讚資訊
                var checkLike = discussionLikeService.GetLikeArrayByEventId(eventId);
                if (checkLike != null)
                    result.LikeArray = checkLike.ToArray();

                //所有留言回覆列表
                var totalMsgList = discussionMsgService.GetAllMsgList(eventId);
                result.TotalCount = totalMsgList.Count();

                return result;
            }
        }

        /// <summary>
        /// 取得主題討論詳細資訊
        /// </summary>
        /// <param name="eventId">活動代碼</param>
        /// <param name="msgEventId">留言代碼 / 回覆代碼</param>
        /// <returns></returns>
        public DiscussionDetail GetDetailByEventId(Guid eventId, Guid? msgEventId = null, int? maxResult = 10)
        {
            var db = _uow.DbContext;

            //取得活動資料
            var sqlData = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (sqlData == null)
                return null;
            else
            {
                //取得主題討論詳細資料
                var discussionInformation = db.ActDiscussion.Where(t => t.EventId == eventId).FirstOrDefault();

                var creatorInfo = db.Members.Find(sqlData.CreateUser);

                var outerKey = Utility.OuterKeyHelper.GuidToPageToken(eventId);
                var result = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionDetail()
                {
                    ModuleKey = "discussion",
                    sOuterKey = outerKey,
                    Publish_Utc = sqlData.Publish_Utc,
                    ToRoomId = sqlData.ToRoomId,
                    Id = sqlData.Id,
                    ActivityDate = sqlData.ActivityDate,
                    StartDate = sqlData.StartDate,
                    Duration = sqlData.Duration,
                    Title = discussionInformation.Name,
                    Content = discussionInformation.Description,
                    Created_Utc = discussionInformation.Created.Utc.Value,
                    Comments=new List<DiscussionMessage>()
                };
                if (sqlData.Deleted != null)
                    result.Deleted_Utc = sqlData.Deleted.Local;

                //查詢留言者資訊
                if (creatorInfo != null)
                {
                    // var creatorInfo = new iThink.Service.Service.MemberService().GetPhotoMember(creatorAccount);
                    result.CreatorAccount = creatorInfo.Account;
                    result.CreatorPhoto = creatorInfo.Photo;
                    result.CreatorName = creatorInfo.Name;
                }

                //查詢主題討論檔案
                var checkFiles = discussionFileService.GetDiscussionFileByEventId(eventId);
                if (checkFiles != null)
                    result.FileList = checkFiles;
                else
                    result.FileList = new List<FileStorageViewModel>();
                //查詢主題討論按讚資訊
                var checkLike = discussionLikeService.GetLikeArrayByEventId(eventId);
                if (checkLike != null)
                    result.LikeArray = checkLike.ToArray();

                var checkMsgList = new List<Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionMessage>();
                //總共留言+回覆筆數
                var msgCount = 0;

                //所有留言回覆列表
                var totalMsgList = discussionMsgService.GetAllMsgList(eventId);
                result.TotalCount = totalMsgList.Count();
                //查詢主題討論的留言
                if (msgEventId == null)
                {
                    //取得主題留言列表
                    checkMsgList = discussionMsgService.GetMessageList(eventId);
                    if (checkMsgList != null)
                    {
                        msgCount = checkMsgList.Count();

                        if (msgCount > maxResult.Value)
                        {
                            result.Comments = checkMsgList.OrderByDescending(t => t.CreateTime).Take(maxResult.Value).Reverse().ToList();
                            result.OlderCount = checkMsgList.Count() - maxResult.Value;
                        }
                        else
                        {
                            result.Comments = checkMsgList.OrderBy(t => t.CreateTime).ToList();
                            result.OlderCount = 0;
                        }
                    }
                }
                //特定留言查詢
                else
                {
                    //判斷msgEventId是否為回覆 true 為回覆 | false 為留言
                    var checkMsgIsReply = (db.ActModuleMessage.FirstOrDefault(t => t.OuterKey == msgEventId.Value && t.Parent != null)) != null ? true : false;
                    if (checkMsgIsReply)
                        //查詢回覆所屬的留言結果
                        checkMsgList = discussionMsgService.GetDiscussionMsgList(eventId, msgEventId.Value);
                    else
                        //特定留言查詢後的列表
                        checkMsgList = discussionMsgService.GetMessageList(eventId, msgEventId);
                    var olderCount = totalMsgList.Where(t => t.Parent == null || t.Parent == 0).Count();
                    result.OlderCount = olderCount - checkMsgList.Count();
                    checkMsgList = checkMsgList.Take(maxResult.Value).ToList();
                    result.Comments = checkMsgList.OrderBy(t => t.CreateTime).ToList();
                }


                return result;
            }
        }

        /// <summary>
        /// 新增一筆主題討論活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="fileCount"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.ActivitysViewModel Add(string circleKey, int memberId, string name, string description, List<Infrastructure.DataTransferObject.RequestFile> files, int tagId)
        {
            var db = _uow.DbContext;
            Guid eveId = Guid.NewGuid();
            DateTime? dt = DateTime.UtcNow;
            int fileCount = files.Count();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey);

            #region // 1. 建立主題討論活動物件                        
            var objDiscussion = new ActDiscussion()
            {
                LearningId = learningCircleInfo.Id,
                Name = name,
                EventId = eveId,
                CreateUser = memberId,
                Created = TimeData.Create(dt),
                Updated = TimeData.Create(null),
                Deleted = TimeData.Create(null),
                Description = description,

                TagId = tagId,
                Visibility = true,
                Enable = true,
                FileCount = fileCount
            };
            #endregion
            #region // 2. 替主題討論活動建立一筆訊息物件
            Activitys objAct = new Activitys()
            {
                Duration = 0,
                ModuleKey = ModuleType.Discussion,
                ToRoomId = circleKey,
                CreateUser = memberId,
                Created = TimeData.Create(dt),
                Updated = TimeData.Create(null),
                Deleted = TimeData.Create(null),
                OuterKey = eveId,
                StartDate = dt,
                IsActivity = true,
                CardisShow = true,
                Publish_Utc = dt
            };
            #endregion

            // 3. 寫入DB
            db.ActDiscussion.Add(objDiscussion);
            db.Activitys.Add(objAct);

            //要先儲存Activity的內容才能生出ActivityId
            db.SaveChanges();
            #region // 3. 替主題討論活動建立一筆互動物件 方便於未來做更多功能而存
            if (fileCount != 0)
            {
                // 2018-03-22 yuschang 建立附加檔案
                var fileStorages = fileService.GetFileStorages(memberId, files);

                //建立主題討論與檔案的關聯
                discussionFileService.DiscussionFileReference(objDiscussion.Id, fileStorages, null);
            }

            #endregion
            return activityService.SignalrResponse(circleKey, objDiscussion.EventId, ModuleType.Discussion, memberId, true);
        }

        /// <summary>
        /// 新增一筆主題討論活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="fileCount"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public ActDiscussion Add(string circleKey, int memberId, string name, string description, int fileCount, int tagId)
        {
            var db = _uow.DbContext;
            Guid eveId = Guid.NewGuid();
            DateTime? dt = DateTime.UtcNow;
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey);

            #region // 1. 建立主題討論活動物件                        
            var objDiscussion = new ActDiscussion()
            {
                LearningId = learningCircleInfo.Id,
                Name = name,
                EventId = eveId,
                CreateUser = memberId,
                Created = TimeData.Create(dt),
                Updated = TimeData.Create(null),
                Deleted = TimeData.Create(null),
                Description = description,

                TagId = tagId,
                Visibility = true,
                Enable = true,
                FileCount = fileCount
            };
            #endregion
            #region // 2. 替主題討論活動建立一筆訊息物件
            Activitys objAct = new Activitys()
            {
                Duration = 0,
                ModuleKey = ModuleType.Discussion,
                ToRoomId = circleKey,
                CreateUser = memberId,
                Created = TimeData.Create(dt),
                Updated = TimeData.Create(null),
                Deleted = TimeData.Create(null),
                OuterKey = eveId,
                StartDate = dt,
                IsActivity = true,
                CardisShow = true,
                Publish_Utc = dt
            };
            #endregion

            // 3. 寫入DB
            db.ActDiscussion.Add(objDiscussion);
            db.Activitys.Add(objAct);

            //要先儲存Activity的內容才能生出ActivityId
            db.SaveChanges();

            return objDiscussion;
        }

        /// <summary>
        /// 編輯主題討論活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public ActDiscussion Update(DiscussionCreateRequestModel data, int memberId, List<Infrastructure.DataTransferObject.RequestFile> files)
        {
            var db = _uow.DbContext;
            DateTime? dt = DateTime.UtcNow;
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(data.CircleKey);
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(data.ActivityOuterKey);
            int fileCount = files.Count;

            var objDiscussion = db.ActDiscussion.FirstOrDefault(t => t.EventId == eventId);
            if (objDiscussion != null)
            {
                objDiscussion.Name = data.Title;
                objDiscussion.Updated = TimeData.Create(DateTime.UtcNow);
                objDiscussion.UpdateUser = memberId;
                objDiscussion.Description = data.Content;
                objDiscussion.FileCount = fileCount;
            }
            db.SaveChanges();

            if (fileCount != 0)
            {
                // 2018-03-22 yuschang 建立附加檔案
                var fileStorages = fileService.GetFileStorages(memberId, files);

                //建立主題討論與檔案的關聯
                var discussionFileReference = discussionFileService.DiscussionFileReference(objDiscussion.Id, fileStorages, null);
            }

            return objDiscussion;
        }

        /// <summary>
        /// 編輯主題討論活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public ActDiscussion Update(Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionCreateRequestModel data, int memberId, int fileCount)
        {
            var db = _uow.DbContext;
            DateTime? dt = DateTime.UtcNow;
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(data.CircleKey);
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(data.ActivityOuterKey);

            var objDiscussion = db.ActDiscussion.FirstOrDefault(t => t.EventId == eventId);
            if (objDiscussion != null)
            {
                objDiscussion.Name = data.Title;
                objDiscussion.Updated = TimeData.Create(DateTime.UtcNow);
                objDiscussion.UpdateUser = memberId;
                objDiscussion.Description = data.Content;
                objDiscussion.FileCount = fileCount;
            }
            db.SaveChanges();
            return objDiscussion;
        }
        /// <summary>
        /// 刪除主題討論活動
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public bool DeleteDiscussion(Guid eventId, int memberId)
        {

            var db = _uow.DbContext;
            var activityInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return false;

            activityInfo.Deleted = TimeData.Create(DateTime.UtcNow);
            activityInfo.DeleteUser = memberId;
            activityInfo.CardisShow = false;
            db.SaveChanges();

            return true;
        }

    }
}


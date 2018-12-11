using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote;
using WiicoApi.Infrastructure.ViewModel.MQTT;
using WiicoApi.Repository;
using WiicoApi.Service.MQTT;

namespace WiicoApi.Service.SignalRService
{
    public class VoteService
    {
        private readonly GenericUnitOfWork _uow;

        public VoteService()
        {
            _uow = new GenericUnitOfWork();
        }
        public IEnumerable<ActVoteItem> GetItemListByCircleKey(string circleKey)
        {
            var list = (from avi in _uow.DbContext.ActVoteitem
                        join av in _uow.DbContext.ActVote on avi.ActVoteId equals av.Id
                        join a in _uow.DbContext.Activitys on av.EventId equals a.OuterKey
                        where a.CardisShow == true && a.IsActivity == true && a.ToRoomId == circleKey
                        select avi).ToList();
            if (list.FirstOrDefault() == null)
                return null;

            return list;


        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public VoteViewModel GetDetail(string outerKey)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            var activityInfo = _uow.ActivitysRepo.GetFirst(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return null;
            var voteInfo = _uow.ActVoteRepo.GetFirst(t => t.EventId == eventId);
            if (voteInfo == null)
                return null;

            var voteItems = _uow.ActVoteItemRepo.Get(t => t.ActVoteId == voteInfo.Id).OrderBy(t => t.Sort).ToList();
            var memberInfo = _uow.MembersRepo.GetFirst(t => t.Id == activityInfo.CreateUser);
            if (memberInfo == null)
                return null;
            var response = new VoteViewModel()
            {
                IsStart = voteInfo.IsStart,
                ModuleKey = activityInfo.ModuleKey,
                Title = voteInfo.Title,
                Description = voteInfo.Content,
                OuterKey = activityInfo.OuterKey,
                sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(activityInfo.OuterKey),
                ToRoomId = activityInfo.ToRoomId,
                Publish_Utc = activityInfo.Publish_Utc,
                VoteItems = voteItems,
                CreatorAccount = memberInfo.Account,
                CreatorName = memberInfo.Name,
                CreatorPhoto = memberInfo.Photo,
                StartDate = activityInfo.StartDate
            };
            var participateCount = 0.00;
            foreach (var item in voteItems)
            {
                participateCount += item.ChooseCount;
            }
            response.PresentCount = voteInfo.PresentCount.HasValue ? voteInfo.PresentCount.Value : Convert.ToInt32(participateCount);

            response.ParticipateRate = (participateCount != 0.00 && response.PresentCount != 0) ? Math.Round((participateCount / Convert.ToDouble(response.PresentCount)) * 100, 0) : 0;
            response.ParticipateCount = Convert.ToInt32(participateCount);
            return response;
        }

        public IEnumerable<VoteListResponse> GetList(string groupId)
        {
            var list = _uow.ActVoteRepo.GetList(groupId);
            if (list != null)
            {
                foreach (var vote in list)
                {
                    vote.PublishTime = vote.PublishTime.ToLocalTime();
                    vote.OuterKey = Utility.OuterKeyHelper.GuidToPageToken(vote.EventId);
                }
            }
            return list;
        }
        /// <summary>
        /// 計算投票項目數量+比例
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="iotDatas"></param>
        /// <returns></returns>
        private IOrderedEnumerable<ActVoteItem> ItemCalculateProxy(IOrderedEnumerable<ActVoteItem> datas, IOTProjectViewModel<IOTProjectRecordData> iotDatas, out double participateCount)
        {
            var chooseIndex = 0;
            participateCount = 0.00;
            //塞投票資訊
            foreach (var item in datas)
            {
                item.ChooseCount = iotDatas.Data[chooseIndex].ChooseCount;
                participateCount += iotDatas.Data[chooseIndex].ChooseCount;
                chooseIndex++;
            }
            //算參與率
            foreach (var item in datas)
            {
                if (participateCount > 0)
                    item.ChooseRate = Math.Round((Convert.ToDouble(item.ChooseCount) / participateCount) * 100, 1);
                else
                    item.ChooseRate = 0;
            }
            return datas;
        }
        /// <summary>
        /// 處理iot取回參數資料
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="iotDatas"></param>
        /// <returns></returns>
        public VoteViewModel ItemProxy(VoteViewModel datas, IOTProjectViewModel<IOTProjectRecordData> iotDatas)
        {
            var participateCount = 0.00;
            //取得在場人數
            datas.PresentCount = Convert.ToInt32(iotDatas.PresentCount);
            var items = ItemCalculateProxy(datas.VoteItems.OrderBy(t => t.Sort), iotDatas, out participateCount);
            datas.VoteItems = items.ToList();
            datas.ParticipateCount = Convert.ToInt32(participateCount);
            //算參與率
            datas.ParticipateRate = (participateCount != 0.00 && datas.PresentCount != 0) ? Math.Round(participateCount / Convert.ToDouble(datas.PresentCount) * 100, 0) : 0;
            return datas;
        }

        /// <summary>
        /// 建立新的投票活動
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="userToken"></param>
        /// <param name="groupId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="voteItems"></param>
        /// <returns></returns>
        public Activitys VoteCreate(int creator, string groupId, string title, string content, List<VoteItemViewModel> voteItems)
        {

            var eventId = Guid.NewGuid();
            var publishDate = DateTime.UtcNow;
            var entity = new ActVote()
            {
                Content = content,
                CreateDateUtc = publishDate,
                Creator = creator,
                EventId = eventId,
                IsStart = false,
                Title = title
            };
            var activityEntity = new Activitys()
            {
                CardisShow = true,
                Created = TimeData.Create(DateTime.UtcNow),
                CreateUser = creator,
                Deleted = TimeData.Create(null),
                Updated = TimeData.Create(null),
                IsActivity = true,
                ModuleKey = Utility.ParaCondition.ModuleType.Vote,
                OuterKey = eventId,
                Publish_Utc = publishDate,
                ToRoomId = groupId
            };
            try
            {
                var vote = _uow.ActVoteRepo.VoteCreate(entity);
                var items = new List<ActVoteItem>();
                var sort = 0;
                foreach (var voteitem in voteItems)
                {
                    var item = new ActVoteItem()
                    {
                        ActVoteId = vote.Id,
                        Content = voteitem.ChooseContent,
                        Title = voteitem.ChooseName,
                        Sort = sort
                    };
                    items.Add(item);
                    sort++;
                }
                var newItems = _uow.ActVoteItemRepo.CreateItems(items);
                var activity = _uow.ActivitysRepo.ActivityCreate(activityEntity);
                return activityEntity;
            }
            catch (Exception ex)
            {

                return null;
                throw ex;
            }
        }

        /// <summary>
        /// 更新資訊
        /// </summary>
        /// <param name="updater"></param>
        /// <param name="groupId"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="voteItems"></param>
        /// <returns></returns>
        public Activitys VoteUpdate(string outerKey, int updater, string title, string content, List<VoteItemViewModel> voteItems)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            var voteInfo = _uow.ActVoteRepo.GetFirst(t => t.EventId == eventId);
            if (voteInfo == null)
                return null;
            var activityInfo = _uow.ActivitysRepo.GetFirst(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return null;
            voteInfo.Title = title;
            voteInfo.Content = content;

            activityInfo.Updated = TimeData.Create(DateTime.UtcNow);
            activityInfo.UpdateUser = updater;

            var voteItemsList = _uow.ActVoteItemRepo.Get(t => t.ActVoteId == voteInfo.Id).ToList();
            foreach (var item in voteItemsList)
            {
                var data = voteItems.FirstOrDefault(t => t.ChooseID == item.Id);

                item.Content = data.ChooseContent;
                item.Sort = data.ChooseSort;
            }

            _uow.SaveChanges();
            return activityInfo;
        }

        /// <summary>
        /// 修改投票活動狀態 - 活動開始 or 活動結束
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="updater"></param>
        /// <param name="isStart"></param>
        /// <returns></returns>
        public VoteStateEnum VoteChangeStart(string circleKey, string outerKey, int updater, bool isStart)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            var voteInfo = _uow.ActVoteRepo.GetFirst(t => t.EventId == eventId);

            var response = VoteStateEnum.Stop;
            if (voteInfo == null)
                return VoteStateEnum.NotFound;
            var activityInfo = _uow.ActivitysRepo.GetFirst(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return VoteStateEnum.NotFound;
            var db = _uow.DbContext;
            //判斷是否有其他的活動已經開始
            var checkOtherIsStart = (from avt in db.ActVote
                                     join a in db.Activitys on avt.EventId equals a.OuterKey
                                     where avt.IsStart == true && a.ToRoomId == circleKey
                                     select avt).FirstOrDefault();

            if (isStart && checkOtherIsStart != null)
                return VoteStateEnum.OtherStart;
            if (voteInfo.IsStart != isStart)
            {
                voteInfo.IsStart = isStart;
                if (isStart)
                {
                    activityInfo.Duration = 9999999;
                    activityInfo.StartDate = DateTime.UtcNow;
                    response = VoteStateEnum.Start;
                }
                else
                {
                    activityInfo.Duration = null;
                    response = VoteStateEnum.Stop;
                }
                activityInfo.Updated = TimeData.Create(DateTime.UtcNow);
                activityInfo.UpdateUser = updater;
                _uow.SaveChanges();
            }
            return response;
        }

        /// <summary>
        /// 儲存投票最後一筆資訊
        /// </summary>
        public VoteViewModel SaveLastVoteData(string outerKey)
        {
            var motionDeviceService = new MotionDeviceService();
            var lastData = motionDeviceService.GetInteractiveData();
            var response = new VoteViewModel();
            if (lastData == null || lastData.Data.FirstOrDefault() == null)
                return null;

            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);

            var participateCount = 0.00;
            if (eventId != null && eventId != Guid.Empty)
            {
                var voteData = _uow.ActVoteRepo.GetFirst(t => t.EventId == eventId);
                if (voteData != null)
                {
                    voteData.PresentCount = Convert.ToInt32(lastData.PresentCount);
                    var voteItemData = _uow.ActVoteItemRepo.Get(t => t.ActVoteId == voteData.Id).ToList().OrderBy(t => t.Sort);
                    if (voteItemData.FirstOrDefault() != null)
                        voteItemData = ItemCalculateProxy(voteItemData, lastData, out participateCount);
                    _uow.SaveChanges();
                }

                response = GetDetail(outerKey);
            }
            return response;
        }

        /// <summary>
        /// 刪除投票資訊
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public bool DeleteVoteData(string outerKey, int memberId)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);

            var voteInfo = _uow.ActVoteRepo.GetFirst(t => t.EventId == eventId);
            if (voteInfo == null)
                return false;

            var activityInfo = _uow.ActivitysRepo.GetFirst(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return false;

            activityInfo.Deleted = TimeData.Create(DateTime.UtcNow);
            activityInfo.DeleteUser = memberId;
            activityInfo.CardisShow = false;
            activityInfo.Duration = null;

            _uow.SaveChanges();
            return true;
        }
    }
}


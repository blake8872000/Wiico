using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SignalRService
{
    public class GroupService
    {
        private readonly GenericUnitOfWork _uow;
        private readonly AuthService authService = new AuthService();

        public GroupService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得分組列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.GroupListViewModel GetGroupListViewModel(string circleKey, int memberId, Guid eventId)
        {
            var db = _uow.DbContext;
            var hasAuth = authService.CheckTokenCircleAuth(memberId, circleKey, "CreateGroupEvent", Utility.ParaCondition.GroupFunctionStatus.Manage.ToString());

            var result = new Infrastructure.ViewModel.GroupListViewModel();
            var groupCategoryInfo = db.ActGroupCategory.Where(t => t.EventId == eventId).FirstOrDefault();
            var learningCircleInfo = db.LearningCircle.Where(t => t.LearningOuterKey == circleKey).FirstOrDefault();
            var groupInfo = db.ActGroup.Where(t => t.CategoryId == groupCategoryInfo.Id);
            var unGroupCount = GetNonGroupMembers(learningCircleInfo.Id, groupCategoryInfo.Id).Count();
            if (hasAuth.Success)
            {
                // var memberId = Convert.ToInt32(hasAuth.Data[0]);
                var sqlResult = _uow.ActivitysRepo.GetGroupListViewModel(circleKey, eventId);
                var groupCount = 0;
                if (sqlResult.Any())
                {
                    groupCount = sqlResult.Count();
                    result = sqlResult.FirstOrDefault();
                    result.GroupCount = groupCount;
                    result.UnGroupCount = unGroupCount;
                }
                else
                {
                    var noGroupResult = db.ActGroupCategory.Where(t => t.EventId == eventId).FirstOrDefault();
                    result.EventId = noGroupResult.EventId;
                    result.GroupTitle = noGroupResult.Name;
                    result.GroupCount = 0;
                    result.CreateDateUtc = noGroupResult.Created.Utc.Value;
                    result.UnGroupCount = unGroupCount;
                }

                return result;
            }
            else
            {
                //  var memberId = Convert.ToInt32(hasAuth.Data[0]);
                var sqlResult = _uow.ActivitysRepo.GetGroupListViewModel(circleKey, memberId, eventId);
                result = sqlResult.FirstOrDefault();
                if (result == null)
                {
                    var sqlNonResult = _uow.ActivitysRepo.GetGroupListViewModelByNongroup(circleKey, eventId);
                    result = sqlNonResult.FirstOrDefault();
                }

                result.GroupCount = groupInfo.Count();
                result.UnGroupCount = unGroupCount;
                result.CreateDateUtc = groupCategoryInfo.Created.Utc.Value;
                return result;
            }
        }

        /// <summary>
        /// 取得未分組成員資訊
        /// </summary>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ValueObject.GroupMember> GetNonGroupMembers(int learningId, int categoryId)
        {
            var db = _uow.DbContext;

            var circleMembers = _uow.CircleMemberRoleplaysRepo.GetGroupMember(learningId, categoryId).ToList();

            var groupMembers = (from agm in db.ActGroupMember
                                join ag in db.ActGroup on agm.GroupId equals ag.Id
                                join agc in db.ActGroupCategory on ag.CategoryId equals agc.Id
                                where agc.Id == categoryId
                                select agm).ToList();
            //刪除已經分組成員
            foreach (var member in groupMembers)
            {
                circleMembers.RemoveAll(t => t.Id == member.MemberId);
            }


            if (circleMembers != null)
                return circleMembers;
            else
                return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class CourseManagerService
    {
        private readonly GenericUnitOfWork _uow ;
        public CourseManagerService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }

        public CourseManagerService(string connectionString=null)
        {
            _uow = new GenericUnitOfWork(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public LearningCircleManager GetDetailByAccountCircleKey(string account, string circleKey)
        {
            var learningInfo = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == circleKey.ToLower());
            if (learningInfo == null)
                return null;
            var memberInfo = _uow.DbContext.Members.FirstOrDefault(t => t.Account.ToLower() == account.ToLower() && t.OrgId == learningInfo.OrgId.Value);
            if (memberInfo == null)
                return null;
            return GetDetail(memberInfo.Id, learningInfo.Id, true);
        }

        public LearningCircleManager GetDetail(int memberId, int circleId, bool? enable)
        {
            if (enable.HasValue)
                return _uow.DbContext.LearningCircleManager.FirstOrDefault(t => t.MemberId == memberId && t.LearningCircleId == circleId && t.Enable == enable.Value);
            else
                return _uow.DbContext.LearningCircleManager.FirstOrDefault(t => t.MemberId == memberId && t.LearningCircleId == circleId);
        }

        public List<CourseManagerGetResponse> GetManagers(string token, string circleKey)
        {
            var db = _uow.DbContext;
            var dbData = from lcm in db.LearningCircleManager
                         join lc in db.LearningCircle on lcm.LearningCircleId equals lc.Id
                         join m in db.Members on lcm.MemberId equals m.Id
                         where lc.LearningOuterKey == circleKey && lcm.Enable == true
                         select new CourseManagerGetResponse()
                         {
                             Account = m.Account,
                             Email = m.Email,
                             Name = m.Name,
                             ResType = lcm.ResType
                         };
            if (dbData.FirstOrDefault() == null)
                return null;

            return dbData.ToList();
        }

        /// <summary>
        /// 根據邀請碼加入課程管理者列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public InviteResponseData CreateMutiple(CourseManagerPostRequest requestData)
        {
            var inviteData = _uow.DbContext.MemberInvite.FirstOrDefault(t => t.Code.ToLower() == requestData.InviteCode.ToLower());
            var responseData = new InviteResponseData()
            {
                InviteStatus = InviteStatusEnum.inviteError
            };
            if (inviteData == null)
                return responseData;

            var learningService = new LearningCircleService();
            var learningCircleInfo = learningService.GetDetailByOuterKey(inviteData.CircleKey.ToLower());
            //課程停止邀請
            if (learningCircleInfo == null || learningCircleInfo.AdminInviteEnable == false || learningCircleInfo.Enable == false)
            {
                responseData.InviteStatus = InviteStatusEnum.EndInvite;
                return responseData;
            }

            //邀請碼失效
            if (inviteData.Enable == false)
            {
                responseData.InviteStatus = InviteStatusEnum.inviteError;
                return responseData;
            }


            responseData.CircleName = learningCircleInfo.Name;

            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(requestData.Token).Result;
            if (memberInfo == null || memberInfo.OrgId != learningCircleInfo.OrgId.Value)
            {
                responseData.InviteStatus = InviteStatusEnum.AccountNotAllow;
                return responseData;
            }
            var organizationInfo = _uow.DbContext.Organizations.FirstOrDefault(t => t.Id == memberInfo.OrgId);
            if (organizationInfo == null)
            {
                responseData.InviteStatus = InviteStatusEnum.AccountNotAllow;
                return responseData;
            }
            responseData.OrgName = organizationInfo.Name;


            //判斷要加的管理者是否跟Token帳號一致
            var checkAddAccountIsSuccess = requestData.Accounts.FirstOrDefault(t => t.ToString() == memberInfo.Account);
            if (checkAddAccountIsSuccess == null)
            {
                responseData.InviteStatus = InviteStatusEnum.inviteError;
                return responseData;
            }
            //將驗證碼失效
            var getOldInviteData = _uow.DbContext.MemberInvite.FirstOrDefault
                                                        (t => t.CircleKey.ToLower() == inviteData.CircleKey.ToLower() &&
                                                         t.Enable == true && t.Type == 1 && t.Code.ToLower() == requestData.InviteCode.ToLower());
            if (getOldInviteData != null)
                getOldInviteData.Enable = false;
            else
            {
                responseData.InviteStatus = InviteStatusEnum.inviteError;
                return responseData;
            }

            var checkJoined = GetDetail(memberInfo.Id, learningCircleInfo.Id, null);
            //判斷是否加入過
            if (checkJoined != null)
            {
                UpdateFromCreate(checkJoined.Id);
                {
                    responseData.InviteStatus = InviteStatusEnum.success;
                    return responseData;
                }
            }
            var entity = new LearningCircleManager()
            {
                CreateUtcDate = DateTime.UtcNow,
                Creator = memberInfo.Id,
                Enable = true,
                LearningCircleId = learningCircleInfo.Id,
                MemberId = memberInfo.Id,
                ResType = requestData.ResType
            };
            _uow.DbContext.LearningCircleManager.Add(entity);
            _uow.SaveChanges();
            responseData.InviteStatus = InviteStatusEnum.success;
            return responseData;
        }

        private bool UpdateFromCreate(int dataId)
        {
            var data = _uow.DbContext.LearningCircleManager.FirstOrDefault(t => t.Id == dataId);
            if (data == null)
                return false;
            else
                data.Enable = true;
            _uow.SaveChanges();
            return true;
        }

        /// <summary>
        /// 刪除管理者
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool DeleteMutiple(CourseManagerDeleteRequest requestData)
        {

            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token);
            if (tokenInfo == null)
                return false;
            if (requestData.Accounts == null || requestData.Accounts.FirstOrDefault() == null)
                return false;
            var db = _uow.DbContext;
            foreach (var account in requestData.Accounts)
            {
                var checkData = GetDetailByAccountCircleKey(account, requestData.CircleKey.ToLower());
                if (checkData == null)
                    continue;
                var data = db.LearningCircleManager.FirstOrDefault(t => t.Id == checkData.Id);
                data.Enable = false;
            }
            db.SaveChanges();
            return true;
        }
    }
}

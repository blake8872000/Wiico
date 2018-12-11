using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Infrastructure.ViewModel.MemberManage;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class CircleMemberService
    {
        private readonly GenericUnitOfWork _uow = new GenericUnitOfWork();

        public CircleMemberService()
        {
            _uow = new GenericUnitOfWork();
        }
        #region 班級成員角色管理

        /// <summary>
        /// 根據使用者帳號與學習圈代碼查詢
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IQueryable<CircleMemberRoleplay> GetDataByAccountCircleKey(string account, string circleKey)
        {
            return _uow.CircleMemberRoleplaysRepo.GetDataByAccountCircleKey(account.ToLower(), circleKey.ToLower());
        }

        /// <summary>
        /// 根據使用者編號與學期圈代碼查詢
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IQueryable<CircleMemberRoleplay> GetDataByMemberIdCircleKey(int memberId, string circleKey)
        {
            return _uow.CircleMemberRoleplaysRepo.GetDataByMemberIdCircleKey(memberId, circleKey);
        }

        /// <summary>
        /// 取得個人在班級上的角色資訊
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public IQueryable<CircleMemberRoleplay> GetDataByMemberIdCircleKeyRoleId(int memberId, string circleKey, int roleId)
        {
            return _uow.CircleMemberRoleplaysRepo.GetDataByMemberIdCircleKeyRoleId(memberId, circleKey.ToLower(), roleId);
        }

        /// <summary>
        /// 取得個人在班級上的角色資訊
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public IQueryable<CircleMemberRoleplay> GetDataByAccountCircleKeyRoleId(string account, string circleKey, int roleId)
        {
            return _uow.CircleMemberRoleplaysRepo.GetDataByAccountCircleKeyRoleId(account, circleKey.ToLower(), roleId);
        }

        /// <summary>
        /// 取得班級成員角色資訊 - 根據使用者編號與學習圈編號與角色編號查詢
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public CircleMemberRoleplay GetCircleMemberRoleInfo(int memberId, int learningCircleId, int roleId)
        {
            var db = _uow.DbContext;
            var data = db.CircleMemberRoleplay.FirstOrDefault(t => t.MemberId == memberId && t.CircleId == learningCircleId && t.RoleId == roleId);
            return data;
        }

        /// <summary>
        /// 取得新版本成員列表結構
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<CircleMemberRoleManageGetResponse> GetAzureCircleMemberRoleListByCircleKey(string circleKey, string token)
        {

            var db = _uow.DbContext;
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return null;
            var checkMemberLevel = (from lr in db.LearningRole
                                    join cmr in db.CircleMemberRoleplay on lr.Id equals cmr.RoleId
                                    where cmr.MemberId == memberInfo.Id
                                    select lr).FirstOrDefault();
            if (checkMemberLevel == null)
                return null;

            var result = new List<CircleMemberRoleManageGetResponse>();
            var roles = (from cm in db.CircleMemberRoleplay
                         join lc in db.LearningCircle on cm.CircleId equals lc.Id
                         join lr in db.LearningRole on cm.RoleId equals lr.Id
                         join m in db.Members on cm.MemberId equals m.Id
                         where lc.LearningOuterKey == circleKey && lr.Enable == true && cm.Enable == true
                         select new CircleMemberRoleManageGetResponse
                         {
                             Account = m.Account,
                             Email = m.Email,
                             IsShowMail = m.IsShowEmail,
                             Name = m.Name,
                             RoleId = lr.Id,
                             RoleName = lr.Name,
                             ResType = cm.ResType,
                             ExternalRid = cm.ExternalRid,
                             Level = lr.Level
                         }).ToList();
            if (roles.FirstOrDefault() == null)
                return null;
            var authService = new AuthService();
            var isDepartmentAdmin = authService.CheckDepartmentAdmin(memberInfo.Id, memberInfo.OrgId);
            foreach (var role in roles)
            {

                if (role.ExternalRid.HasValue)
                {
                    role.IsEdit = false;
                    continue;
                }
                if ((isDepartmentAdmin == true || memberInfo.IsOrgAdmin == true))
                {
                    role.IsEdit = true;
                    continue;
                }
                if (checkMemberLevel.Level.Value < role.Level.Value)
                    role.IsEdit = true;
                else
                    role.IsEdit = false;
            }
            return roles.ToList();
        }

        /// <summary>
        /// 取得班級成員角色列表 - 舊的APP使用
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<CircleMemberRoleInfo> GetCircleMemberRoleListByCircleKey(string circleKey)
        {
            var db = _uow.DbContext;
            var result = new List<CircleMemberRoleInfo>();
            var roles = (from lr in db.LearningRole
                         join lc in db.LearningCircle on lr.LearningId equals lc.Id
                         where lc.LearningOuterKey == circleKey && lr.Enable == true
                         select lr).ToList();

            if (roles.FirstOrDefault() == null)
                return null;

            foreach (var role in roles)
            {
                var data = new CircleMemberRoleInfo();
                data.RoleInfo = role;
                var memberList = (from cmr in db.CircleMemberRoleplay
                                  join m in db.Members on cmr.MemberId equals m.Id
                                  join lr in db.LearningRole on cmr.RoleId equals lr.Id
                                  join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                                  where cmr.RoleId == role.Id && lc.LearningOuterKey == circleKey && cmr.Enable == true
                                  select m).ToList();

                data.MemberList = memberList.FirstOrDefault() != null ? memberList : null;
                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// 根據InviteCode 建立成員角色資訊 - 邀請碼加入
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public InviteResponseData InsertCircleMemberRoleByInvite(CircleMemberRoleRequest requestData)
        {
            var db = _uow.DbContext;

            var inviteData = db.MemberInvite.FirstOrDefault(t => t.Code.ToLower() == requestData.InviteCode.ToLower());
            var responseData = new InviteResponseData()
            {
                InviteStatus = InviteStatusEnum.inviteError
            };
            if (inviteData == null)
                return responseData;

            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == inviteData.CircleKey.ToLower());
            if (learningCircleInfo == null || learningCircleInfo.InviteEnable == false || learningCircleInfo.Enable == false)
            {
                responseData.InviteStatus = InviteStatusEnum.EndInvite;
                return responseData;
            }
            responseData.CircleName = learningCircleInfo.Name;

            if (inviteData.Enable == false)
            {
                responseData.InviteStatus = InviteStatusEnum.inviteError;
                return responseData;
            }

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

            //查出學生角色資訊
            var learningRoleInfo = db.LearningRole.FirstOrDefault(t => t.Level == 3 && t.LearningId == learningCircleInfo.Id && t.IsFixed == false);
            if (learningRoleInfo == null)
            {
                responseData.InviteStatus = InviteStatusEnum.EndInvite;
                return responseData;
            }

            //判斷要加的管理者是否跟Token帳號一致
            var checkAddAccountIsSuccess = requestData.Accounts.FirstOrDefault(t => t.ToString().ToLower() == memberInfo.Account.ToLower());
            if (checkAddAccountIsSuccess == null)
            {
                responseData.InviteStatus = InviteStatusEnum.inviteError;
                return responseData;
            }
            //將驗證碼失效
            var getOldInviteData = _uow.DbContext.MemberInvite.FirstOrDefault
                                                        (t => t.CircleKey.ToLower() == inviteData.CircleKey.ToLower() &&
                                                         t.Enable == true &&
                                                         t.Type == 0 &&
                                                         t.Code.ToLower() == requestData.InviteCode.ToLower());
            if (getOldInviteData != null && getOldInviteData.IsCourseCode == false)
                getOldInviteData.Enable = false;
            else if (getOldInviteData.IsCourseCode == false)
            {
                responseData.InviteStatus = InviteStatusEnum.inviteError;
                return responseData;
            }
            var checkJoined = db.CircleMemberRoleplay.FirstOrDefault(t => t.MemberId == memberInfo.Id && t.RoleId == learningRoleInfo.Id && t.CircleId == learningCircleInfo.Id);
            if (checkJoined == null)
            {
                var entity = new CircleMemberRoleplay()
                {
                    CircleId = learningCircleInfo.Id,
                    Enable = true,
                    MemberId = memberInfo.Id,
                    ResType = requestData.ResType,
                    RoleId = learningRoleInfo.Id
                };
                db.CircleMemberRoleplay.Add(entity);
                var circleMemberEntity = new CircleMember()
                {
                    CircleId = learningCircleInfo.Id,
                    Enabled = true,
                    MemberId = memberInfo.Id,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null)
                };
                db.CircleMember.Add(circleMemberEntity);
                db.CircleMemberRoleplay.Add(entity);
            }
            else
                checkJoined.Enable = true;
            db.SaveChanges();
            responseData.InviteStatus = InviteStatusEnum.success;
            return responseData;
        }

        /// <summary>
        /// 修改多筆資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool UpdateMutiple(CircleMemberRolePutRequest requestData, bool? isAdmin = false)
        {
            var memberService = new MemberService();
            var updaterInfo = memberService.TokenToMember(requestData.Token).Result;
            if (updaterInfo == null)
                return false;
            if (requestData.Accounts == null || requestData.Accounts.FirstOrDefault() == null)
                return false;

            var learningRoleService = new LearningRoleService();
            var updateRoleLevel = learningRoleService.GetMyRole(requestData.Token, requestData.CircleKey.ToLower());
            if (updateRoleLevel == null)
                return false;

            //判斷是否能改request的role
            var checkRoleLevel = learningRoleService.GetDetailById(requestData.RoleId);
            if (checkRoleLevel == null)
                return false;

            if (isAdmin.Value == false)
            {
                //設定數字越小，權限越大；故如果level大於等於request的level，就不能修改
                if (updateRoleLevel.Level >= checkRoleLevel.Level)
                    return false;
            }


            foreach (var updateAccount in requestData.Accounts)
            {
                //該成員已經有該角色
                var checkData = GetDataByAccountCircleKeyRoleId(updateAccount, requestData.CircleKey.ToLower(), requestData.RoleId).FirstOrDefault();
                if (checkData != null)
                {
                    checkData.Enable = true;
                    continue;
                }
                //修改該成員的角色
                var oldData = GetDataByAccountCircleKey(updateAccount, requestData.CircleKey.ToLower()).FirstOrDefault();

                if (oldData != null)
                    oldData.RoleId = requestData.RoleId;
                else
                { //沒有就新增一筆
                    InsertCircleMemberRole(requestData.CircleKey.ToLower(), updateAccount, requestData.Token, requestData.RoleId);
                }
            }
            _uow.SaveChanges();
            return true;
        }

        /// <summary>
        /// 編輯單筆資料 -根據帳號編輯
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="account"></param>
        /// <param name="token"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool UpdateCircleMemberRole(string circleKey, string account, string token, int roleId)
        {
            var checkData = GetDataByAccountCircleKeyRoleId(account, circleKey.ToLower(), roleId).FirstOrDefault();
            if (checkData != null)
                checkData.Enable = true;
            else
            {
                var oldData = GetDataByAccountCircleKey(account, circleKey.ToLower()).FirstOrDefault();
                oldData.RoleId = roleId;
            }
            return true;
        }

        /// <summary>
        /// 新增單筆資訊
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="account"></param>
        /// <param name="token"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool InsertCircleMemberRole(string circleKey, string account, string token, int roleId)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return false;

            var learningRoleService = new LearningRoleService();
            var learningRoleInfo = learningRoleService.GetDetailById(roleId);
            if (learningRoleInfo == null)
                return false;
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey.ToLower());
            if (learningCircleInfo == null)
                return false;
            var accountInfo = memberService.AccountToMember(account, learningCircleInfo.OrgId);
            if (accountInfo == null)
                return false;

            var entity = new CircleMemberRoleplay()
            {
                CircleId = learningCircleInfo.Id,
                Enable = true,
                MemberId = accountInfo.Id,
                RoleId = roleId,
                ResType = 0,
                ExternalRid = null
            };
            _uow.DbContext.CircleMemberRoleplay.Add(entity);
            return true;
        }

        /// <summary>
        /// 建立成員角色資訊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool InsertMutipleCircleMemberRole(CircleMemberRoleRequest data)
        {
            var db = _uow.DbContext;
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(data.Token).Result;
            if (tokenInfo == null)
                return false;

            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == data.CircleKey);
            if (learningCircleInfo == null)
                return false;

            foreach (var account in data.Accounts)
            {
                var memberService = new MemberService();
                var memberInfo = memberService.AccountToMember(account.ToLower(), learningCircleInfo.OrgId.Value);

                if (memberInfo == null)
                    return false;

                //判斷是否已存在關聯
                var checkData = GetCircleMemberRoleInfo(memberInfo.Id, learningCircleInfo.Id, data.RoleId.Value);

                //已存在就不新增
                if (checkData != null)
                    continue;

                var entity = new Infrastructure.Entity.CircleMemberRoleplay()
                {
                    Enable = true,
                    CircleId = learningCircleInfo.Id,
                    MemberId = memberInfo.Id,
                    RoleId = data.RoleId.Value
                };

                var circlememberEntity = new CircleMember()
                {
                    MemberId = memberInfo.Id,
                    CircleId = learningCircleInfo.Id,
                    Enabled = true,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    CreateUser = tokenInfo.MemberId
                };
                db.CircleMemberRoleplay.Add(entity);
                db.CircleMember.Add(circlememberEntity);
            }
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 編輯成員選課資訊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public bool UpdateCircleMemberRole(CircleMemberRoleRequest data)
        {
            var db = _uow.DbContext;
            var updater = (from m in db.Members
                           join ut in db.UserToken on m.Id equals ut.MemberId
                           where ut.Token == data.Token
                           select m).FirstOrDefault();

            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == data.CircleKey);
            if (learningCircleInfo == null)
                return false;
            foreach (var account in data.Accounts)
            {

                var memberInfo = db.Members.FirstOrDefault(t => t.Account.ToLower() == account.ToLower());
                if (memberInfo == null)
                    return false;

                //判斷是否已存在關聯
                var checkData = db.CircleMemberRoleplay.FirstOrDefault(t => t.MemberId == memberInfo.Id && t.CircleId == learningCircleInfo.Id);
                if (checkData != null)
                {
                    checkData.RoleId = data.RoleId.Value;
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 刪除成員選課資訊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteCircleMemberRole(CircleMemberRolePutRequest data)
        {
            var db = _uow.DbContext;
            var tokenService = new TokenService();

            var deleter = tokenService.GetTokenInfo(data.Token).Result;
            if (deleter == null)
                return false;
            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == data.CircleKey);
            if (learningCircleInfo == null)
                return false;

            foreach (var account in data.Accounts)
            {
                var memberInfo = db.Members.FirstOrDefault(t => t.Account.ToLower() == account.ToLower());
                if (memberInfo == null)
                    return false;
                //判斷是否已存在關聯
                var checkRoleData = GetDataByAccountCircleKey(account, data.CircleKey.ToLower());
                var checkCircleMemberData = GetCircleMemberInfo(memberInfo.Id, learningCircleInfo.Id);
                if (checkRoleData.FirstOrDefault() != null)
                {

                    if (checkRoleData.FirstOrDefault().ExternalRid == null)
                        db.CircleMemberRoleplay.RemoveRange(checkRoleData.ToList());
                    else
                        return false;
                }
                if (checkCircleMemberData != null)
                    db.CircleMember.Remove(checkCircleMemberData);
            }
            db.SaveChanges();
            return true;
        }
        #endregion

        #region 班級成員管理

        /// <summary>
        /// 取得選課列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.Backend.CircleMemberInfo> GetCircleMemberListByCircleKey(string circleKey)
        {
            var db = _uow.DbContext;
            var data = (from cm in db.CircleMember
                        join lc in db.LearningCircle on cm.CircleId equals lc.Id
                        join m in db.Members on cm.MemberId equals m.Id
                        where lc.LearningOuterKey == circleKey
                        select new CircleMemberInfo
                        {
                            LearningCircleInfo = lc,
                            MemberInfo = m
                        }).ToList();
            if (data.FirstOrDefault() == null)
                return null;

            return data;
        }

        /// <summary>
        /// 取得選課資訊
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public CircleMember GetCircleMemberInfo(int memberId, int learningCircleId)
        {
            var db = _uow.DbContext;
            var data = db.CircleMember.FirstOrDefault(t => t.MemberId == memberId && t.CircleId == learningCircleId);
            return data;
        }

        /// <summary>
        /// 建立選課資訊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool InsertCircleMember(CircleMemberRequest data)
        {
            var db = _uow.DbContext;
            var creator = (from m in db.Members
                           join ut in db.UserToken on m.Id equals ut.MemberId
                           where ut.Token == data.Token
                           select m).FirstOrDefault();

            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == data.CircleKey);
            if (learningCircleInfo == null)
                return false;
            var memberInfo = db.Members.FirstOrDefault(t => t.Account == data.Account);
            if (memberInfo == null)
                return false;

            //判斷是否已存在關聯
            var checkData = GetCircleMemberInfo(memberInfo.Id, learningCircleInfo.Id);
            if (checkData != null)
            {
                checkData.Enabled = true;
                db.SaveChanges();
                return true;
            }

            var entity = new Infrastructure.Entity.CircleMember()
            {
                CircleId = learningCircleInfo.Id,
                Created = TimeData.Create(DateTime.UtcNow),
                Updated = TimeData.Create(null),
                Deleted = TimeData.Create(null),
                CreateUser = creator.Id,
                Enabled = true,
                MemberId = memberInfo.Id
            };
            db.CircleMember.Add(entity);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 編輯成員選課資訊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public bool UpdateCircleMember(CircleMemberRequest data)
        {
            var db = _uow.DbContext;
            var updater = (from m in db.Members
                           join ut in db.UserToken on m.Id equals ut.MemberId
                           where ut.Token == data.Token
                           select m).FirstOrDefault();

            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == data.CircleKey);
            if (learningCircleInfo == null)
                return false;
            var memberInfo = db.Members.FirstOrDefault(t => t.Account == data.Account);
            if (memberInfo == null)
                return false;

            //判斷是否已存在關聯
            var checkData = GetCircleMemberInfo(memberInfo.Id, learningCircleInfo.Id);
            if (checkData != null)
            {
                checkData.Enabled = data.Enable;
                db.SaveChanges();
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// 刪除成員選課資訊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DeleteCircleMember(CircleMemberRequest data)
        {
            var db = _uow.DbContext;
            var deleter = (from m in db.Members
                           join ut in db.UserToken on m.Id equals ut.MemberId
                           where ut.Token == data.Token
                           select m).FirstOrDefault();

            var learningCircleInfo = db.LearningCircle.FirstOrDefault(t => t.LearningOuterKey == data.CircleKey);
            if (learningCircleInfo == null)
                return false;
            var memberInfo = db.Members.FirstOrDefault(t => t.Account == data.Account);
            if (memberInfo == null)
                return false;

            //判斷是否已存在關聯
            var checkData = GetCircleMemberInfo(memberInfo.Id, learningCircleInfo.Id);
            if (checkData != null)
            {
                db.CircleMember.Remove(checkData);
                db.SaveChanges();
                return true;
            }
            else
                return false;

        }

        #endregion

    }
}

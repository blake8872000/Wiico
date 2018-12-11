using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class LearningRoleService
    {
        private readonly GenericUnitOfWork _uow;

        public LearningRoleService()
        {
            _uow = new GenericUnitOfWork();
        }

        public LearningRole GetDetailById(int roleId)
        {
            var data = _uow.DbContext.LearningRole.FirstOrDefault(t => t.Id == roleId);
            return data;
        }

        /// <summary>
        /// 取得自己在學習圈內的角色
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public LearningRole GetMyRole(string token, string circleKey)
        {
            var db = _uow.DbContext;
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return null;
            var responseData = (from cmr in db.CircleMemberRoleplay
                                join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                                join lr in db.LearningRole on cmr.RoleId equals lr.Id
                                where lc.LearningOuterKey.ToLower() == circleKey.ToLower() && cmr.MemberId == tokenInfo.MemberId
                                select lr).FirstOrDefault();
            return responseData;
        }

        public IEnumerable<LearningRole> GetListByCircleKey(string circleKey)
        {
            var db = _uow.DbContext;
            var responseData = (from lr in db.LearningRole
                                join lc in db.LearningCircle on lr.LearningId equals lc.Id
                                where lc.LearningOuterKey.ToLower() == circleKey.ToLower()
                                select lr);
            if (responseData.FirstOrDefault() == null)
                return null;
            return responseData.ToList();
        }

        /// <summary>
        /// 取得角色列表 - 根據circleKey[新結構]
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<LearningRoleGetResponse> GetLearningRolesByCircleKey(string circleKey, string token)
        {
            var db = _uow.DbContext;
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey);
            if (learningCircleInfo == null)
                return null;

            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return null;
            var checkTokenLevel = (from lr in db.LearningRole
                                   join cmr in db.CircleMemberRoleplay on lr.Id equals cmr.RoleId
                                   where cmr.CircleId == learningCircleInfo.Id && cmr.MemberId == memberInfo.Id
                                   select lr).FirstOrDefault();
            if (checkTokenLevel == null)
                return null;

            var responseData = (from lr in db.LearningRole
                                join lc in db.LearningCircle on lr.LearningId equals lc.Id
                                where lc.LearningOuterKey == circleKey && lr.Enable == true
                                select new LearningRoleGetResponse
                                {
                                    Level = lr.Level,
                                    Name = lr.Name,
                                    RoleCode = lr.Ican5Memo,
                                    Id = lr.Id,
                                    IsFixed = lr.IsFixed,
                                    ExternalRid = lr.ExternalRid
                                }).ToList();

            if (responseData.FirstOrDefault() == null)
                return null;
            var authService = new AuthService();
            var isDepartmentAdmin = authService.CheckDepartmentAdmin(memberInfo.Id, memberInfo.OrgId);
            foreach (var data in responseData)
            {
                if (data.ExternalRid.HasValue)
                {
                    data.IsEdit = false;
                    continue;
                }
                if ((isDepartmentAdmin == true || memberInfo.IsOrgAdmin == true))
                {
                    data.IsEdit = true;
                    continue;
                }
                if (checkTokenLevel.Level.Value < data.Level.Value)
                    data.IsEdit = true;
                else
                    data.IsEdit = false;
            }
            return responseData;
        }

        /// <summary>
        /// 建立多筆
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public IEnumerable<LearningRole> CreateMutiple(LearningRolePostResquest requestData)
        {
            var db = _uow.DbContext;
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null)
                return null;
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(requestData.CircleKey.ToLower());
            if (learningCircleInfo == null)
                return null;

            var responseData = new List<LearningRole>();
            foreach (var data in requestData.Roles)
            {
                var checkEmpty = db.LearningRole.FirstOrDefault(t => t.Name == data.Name && t.Level == data.Level && t.LearningId == learningCircleInfo.Id);
                if (checkEmpty != null)
                    continue;
                var entity = new LearningRole()
                {
                    Created = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    IsAdminRole = data.Level <= 2 ? true : false,
                    Name = data.Name,
                    Level = data.Level,
                    Enable = true,
                    IsFixed = true,
                    LearningId = learningCircleInfo.Id,
                    CreateUser = tokenInfo.MemberId
                };
                db.LearningRole.Add(entity);
                responseData.Add(entity);
            }
            db.SaveChanges();
            return responseData;
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool UpdateMutiple(LearningRolePostResquest requestData)
        {

            if (requestData.Roles.FirstOrDefault() == null)
                return false;

            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null)
                return false;

            var db = _uow.DbContext;
            foreach (var learningRole in requestData.Roles)
            {
                var oldLeanirngRole = db.LearningRole.Find(learningRole.RoleId);
                //查不到或無法修改就跳過
                if (oldLeanirngRole == null || oldLeanirngRole.IsFixed == false)
                    continue;
                oldLeanirngRole.Name = learningRole.Name;
                oldLeanirngRole.Level = learningRole.Level;
                oldLeanirngRole.Updated = TimeData.Create(DateTime.UtcNow);
                oldLeanirngRole.UpdateUser = tokenInfo.MemberId;
            }
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool DeleteMutiple(LearningRoleDeleteResquest requestData)
        {

            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null)
                return false;

            var db = _uow.DbContext;
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(requestData.CircleKey.ToLower());
            if (learningCircleInfo == null)
                return false;
            var circleMemberService = new Service.Backend.CircleMemberService();
            //查出該課程的學生角色
            var studentRoleInfo = db.LearningRole.FirstOrDefault(t => t.Level == 3 && t.LearningId == learningCircleInfo.Id && t.IsFixed == false);
            if (studentRoleInfo == null)
                return false;
            foreach (var learningRoleId in requestData.Ids)
            {
                var oldLeanirngRole = db.LearningRole.Find(learningRoleId);
                //查不到或無法修改就跳過
                if (oldLeanirngRole == null || oldLeanirngRole.IsFixed == false)
                    continue;
                //將該角色的所有成員全部移至學生Level3 isFixed=false的角色中
                var circleMemberDatas = db.CircleMemberRoleplay.Where(t => t.RoleId == learningRoleId && t.CircleId == oldLeanirngRole.LearningId);
                foreach (var circleMemberData in circleMemberDatas)
                {
                    circleMemberData.RoleId = studentRoleInfo.Id;
                }
                oldLeanirngRole.Enable = false;
                oldLeanirngRole.Deleted = TimeData.Create(DateTime.UtcNow);
                oldLeanirngRole.DeleteUser = tokenInfo.MemberId;
            }
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 建立課程組織預設角色 - 目前給同步API用
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public List<LearningRole> SetLearningCircleOrgnaizationRole(int orgId, int learningCircleId)
        {
            var db = _uow.DbContext;
            var responseData = new List<LearningRole>();
            var templateRoleDatas = db.LearningTemplateRoles.Where(t => t.OrgId == orgId);
            if (templateRoleDatas.FirstOrDefault() == null)
            {
                return null;
            }
            foreach (var templateRole in templateRoleDatas)
            {
                var learningCircleRoleEntity = new LearningRole()
                {
                    ExternalRid = 1,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    CreateUser = 1,
                    Enable = true,
                    IsFixed = true,
                    Ican5Memo = templateRole.RoleKey,
                    LearningId = learningCircleId,
                    Name = templateRole.Name,
                    Level = templateRole.Level
                };
                learningCircleRoleEntity.IsAdminRole = (templateRole.Level <= 2) ? true : false;
                db.LearningRole.Add(learningCircleRoleEntity);
                responseData.Add(learningCircleRoleEntity);
            }
            db.SaveChanges();
            return responseData;
        }

        /// <summary>
        /// 取得學習圈成員 - 以角色為主 [角色內包使用者]
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.BusinessObject.LearningCircleMemberList> GetLearningCircleMemberList(string circleKey)
        {

            var result = new List<Infrastructure.BusinessObject.LearningCircleMemberList>();
            var db = _uow.DbContext;
            //取得角色清單
            var roles = (from lr in db.LearningRole
                         join lc in db.LearningCircle on lr.LearningId equals lc.Id
                         where lc.LearningOuterKey.Equals(circleKey) && lr.Enable == true
                         orderby lr.Ican5Memo descending
                         select lr).ToList();

            //塞角色成員清單
            foreach (var role in roles)
            {
                var queryAdmin = SetRoleMember(role);
                result.Add(queryAdmin);
            }
            return result;
        }

        /// <summary>
        /// 取得學習圈成員 - 以使用者為主 [使用者內包角色]
        /// </summary>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ValueObject.AuthMember> GetMemberList(int learningId)
        {
            var result = new List<Infrastructure.ValueObject.AuthMember>();
            var db = _uow.DbContext;
            var query = from lr in db.LearningRole
                        join cmr in db.CircleMemberRoleplay on lr.Id equals cmr.RoleId

                        join m in db.Members on cmr.MemberId equals m.Id
                        where lr.LearningId.Equals(learningId)
                        group m.Id by m into g
                        select g;
            foreach (var _item in query)
            {
                var resMemberInfo = new Infrastructure.ValueObject.AuthMember();
                resMemberInfo.Account = _item.Key.Account;
                resMemberInfo.AccountId = _item.Key.Id;
                resMemberInfo.AccountName = _item.Key.Name;
                resMemberInfo.Picture = _item.Key.Photo;
                result.Add(resMemberInfo);
            }
            return result;
        }

        /// <summary>
        /// 取得某個使用者在某個學習圈的角色
        /// </summary>
        /// <param name="memberId">要查詢的使用者代碼</param>
        /// <param name="learningId">學習圈代碼</param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.LearningRole GetLearningMemberRole(int memberId, int learningId)
        {
            var result = new Infrastructure.BusinessObject.LearningRole();
            result.RoleInfo = new List<Infrastructure.Entity.LearningRole>();
            var db = _uow.DbContext;
            var sqlSectionRole = db.CircleMemberRoleplay.Where(t => t.MemberId.Equals(memberId));


            foreach (var _resitem in sqlSectionRole)
            {
                //取得角色
                var resCourseRole = db.LearningRole.Find(_resitem.RoleId);
                if (resCourseRole != null)
                {
                    //取得某個學習圈的角色
                    if (resCourseRole.LearningId.Equals(learningId))
                    {
                        resCourseRole.Enable = _resitem.Enable;
                        result.RoleInfo.Add(resCourseRole);
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// 取得某個學習圈的所有角色
        /// </summary>
        /// <param name="learningCircleId"></param>
        /// <returns></returns>
        public IEnumerable<LearningRole> GetLearningCircleRoles(int learningCircleId)
        {
            var result = _uow.DbContext.LearningRole.Where(t => t.LearningId == learningCircleId && t.Enable == true).ToList();
            return result;
        }

        /// <summary>
        /// 取得某個學習圈的所有角色
        /// </summary>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.LearningRole GetLearningRoleList(int learningId)
        {
            var result = new Infrastructure.BusinessObject.LearningRole();
            result.RoleInfo = new List<Infrastructure.Entity.LearningRole>();
            var db = _uow.DbContext;
            var query = from lr in db.LearningRole
                        join lc in db.LearningCircle on lr.LearningId equals lc.Id
                        where lc.Id.Equals(learningId) && lr.Enable.Equals(true)
                        select lr;
            foreach (var _item in query)
            {
                //取得角色
                var resCourseRole = db.LearningRole.Find(_item.Id);
                if (resCourseRole != null)
                {
                    //取得某個學習圈的角色
                    if (resCourseRole.LearningId.Equals(learningId))
                    {
                        result.RoleInfo.Add(resCourseRole);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 取得非管理員角色
        /// </summary>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.LearningRole GetLearningNormalRoleList(int learningId)
        {
            var result = new Infrastructure.BusinessObject.LearningRole();
            result.RoleInfo = new List<Infrastructure.Entity.LearningRole>();
            var db = _uow.DbContext;
            var query = from lr in db.LearningRole
                        join lc in db.LearningCircle on lr.LearningId equals lc.Id
                        where lc.Id.Equals(learningId) && !(lr.IsAdminRole.Equals(true) && lr.Enable.Equals(true))
                        select lr;
            foreach (var _item in query)
            {
                //取得角色
                var resCourseRole = db.LearningRole.Find(_item.Id);
                if (resCourseRole != null)
                {
                    //取得某個學習圈的角色
                    if (resCourseRole.LearningId.Equals(learningId))
                    {
                        result.RoleInfo.Add(resCourseRole);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 塞入角色成員清單 - select
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleName"></param>
        /// <param name="isAdminRole"></param>
        /// <returns></returns>
        private Infrastructure.BusinessObject.LearningCircleMemberList SetRoleMember(Infrastructure.Entity.LearningRole roleInfo)
        {
            var db = _uow.DbContext;
            //角色所屬成員類別
            var resAdminMemberListInfo = new Infrastructure.BusinessObject.LearningCircleMemberList();
            resAdminMemberListInfo.MemberInfo = new List<Infrastructure.Entity.Member>();
            roleInfo.Created = null;
            roleInfo.Deleted = null;
            roleInfo.Updated = null;
            resAdminMemberListInfo.RoleInfo = roleInfo;
            //針對角色匯入成員

            #region //2016/9/12 mark by sophiee:為顯示舊ican個人大頭照，不讀取members table
            //成員query
            var queryMember = (from m in db.Members
                               join cmr in db.CircleMemberRoleplay on m.Id equals cmr.MemberId
                               join lr in db.LearningRole on cmr.RoleId equals lr.Id
                               orderby m.Account
                               where lr.Id == roleInfo.Id
                               select m).ToList();
            #endregion

            // var queryMember = _uow.IThinkVmRepo.GetTempMember(roleId, isAdminRole);
            if (queryMember.Count() > 0)
            {
                //塞入成員
                foreach (var member in queryMember)
                {
                    member.PassWord = new byte[0];
                    member.Created = null;
                    member.Updated = null;
                    member.Deleted = null;
                    resAdminMemberListInfo.MemberInfo.Add(member);
                }
            }
            return resAdminMemberListInfo;
        }

        /// <summary>
        /// 取得某個學習圈內使用者的角色資訊 - 是否為某個角色[enable]
        /// </summary>
        public IEnumerable<Infrastructure.ViewModel.MemberRoleList> GetLearningMemberRoleEnableList(int learningId)
        {
            var db = _uow.DbContext;
            var result = new List<Infrastructure.ViewModel.MemberRoleList>();
            //取得某個學習圈內所有角色
            var sqlRole = from lr in db.LearningRole
                          where lr.LearningId.Equals(learningId)
                          orderby lr.Id
                          select lr;
            var sqlMember = from cmr in db.CircleMemberRoleplay

                            join lr in db.LearningRole on cmr.RoleId equals lr.Id
                            where lr.LearningId.Equals(learningId)
                            group cmr by cmr.MemberId into g
                            orderby g.Key
                            select g;

            //取得某個學習圈的角色設定狀態
            var sqlMemberStatus = from cmr in db.CircleMemberRoleplay

                                  join lr in db.LearningRole on cmr.RoleId equals lr.Id
                                  where lr.LearningId.Equals(learningId) && lr.Enable.Equals(true) && cmr.Enable.Equals(true)
                                  orderby cmr.MemberId
                                  select new
                                  {
                                      RoleId = lr.Id,
                                      RoleName = lr.Name,
                                      MemberId = cmr.MemberId
                                  };


            //取得所有使用者角色狀態
            foreach (var _itemMember in sqlMember)
            {
                var resMemberRoleinfo = new Infrastructure.ViewModel.MemberRoleList();
                resMemberRoleinfo.RoleInfo = new List<Infrastructure.BusinessObject.MemberRoleInfo>();
                resMemberRoleinfo.MemberId = _itemMember.Key;

                //取得使用者角色狀態
                foreach (var _itemRole in sqlRole)
                {

                    var checkRole = sqlMemberStatus.Where(t => t.MemberId.Equals(_itemMember.Key) && t.RoleId.Equals(_itemRole.Id));
                    var resRoleInfo = new Infrastructure.BusinessObject.MemberRoleInfo();
                    resRoleInfo.RoleId = _itemRole.Id;
                    resRoleInfo.RoleName = _itemRole.Name;

                    //判斷使用者是否有該角色
                    if (checkRole.Any())
                        resRoleInfo.Enable = true;
                    else
                        resRoleInfo.Enable = false;

                    //判斷是否為預設角色 - 是就可以修改
                    if (_itemRole.ExternalRid != null)
                    {
                        resRoleInfo.Disabled = false;
                    }
                    else
                    {
                        resRoleInfo.Disabled = true;
                    }

                    resMemberRoleinfo.RoleInfo.Add(resRoleInfo);
                }
                result.Add(resMemberRoleinfo);
            }
            return result;
        }

        /// <summary>
        /// 新增自訂角色
        /// </summary>
        /// <param name="memberId">管理者代碼</param>
        /// <param name="learningId">學習圈代碼</param>
        /// <param name="roleName">自訂角色名稱</param>
        /// <param name="isAdmin">是否為管理員</param>
        /// <returns></returns>
        public Infrastructure.Entity.LearningRole AddLearningEditRole(int memberId, int learningId, string roleName, bool isAdmin, bool? isFixed = false, int? level = 3)
        {
            var db = _uow.DbContext;
            var info = new LearningRole
            {
                LearningId = learningId,
                Name = roleName,
                IsFixed = isFixed.Value,
                IsAdminRole = isAdmin,
                Created = Infrastructure.Property.TimeData.Create(DateTime.UtcNow),
                CreateUser = memberId,
                Enable = true,
                UpdateUser = null,
                DeleteUser = null,
                Updated = Infrastructure.Property.TimeData.Create(null),
                Deleted = Infrastructure.Property.TimeData.Create(null),
                Level = level.Value,
                Ican5Memo = "1000"
            };
            if (level.Value < 3)
                info.Ican5Memo = level.Value == 1 ? "2000" : "2010";
            db.LearningRole.Add(info);
            db.SaveChanges();
            return info;
        }

        /// <summary>
        /// 設定學習圈單人角色關聯
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="roleId"></param>
        /// <returns>回傳設定角色資訊</returns>
        public Infrastructure.Entity.CircleMemberRoleplay SetMemberRoleInfo(int learningId, int memberId, int roleId, int adminUser, bool setEnable)
        {
            var db = _uow.DbContext;
            //判斷是否已設定過角色
            var checkRole = db.CircleMemberRoleplay.Where(t => t.RoleId.Equals(roleId) && t.MemberId.Equals(memberId) && t.CircleId.Equals(learningId));
            var entity = new CircleMemberRoleplay();

            //直接修改角色
            if (checkRole.Any())
            {
                entity = checkRole.ToList()[0];
                entity.Enable = setEnable;

                db.SaveChanges();
            }
            //新增關聯角色
            else
            {
                entity = new CircleMemberRoleplay
                {
                    RoleId = roleId,
                    CircleId = learningId,
                    MemberId = memberId,
                    Enable = true,
                };
                db.CircleMemberRoleplay.Add(entity);
                db.SaveChanges();
            }
            var result = db.CircleMemberRoleplay.Find(entity.Id);
            return result;
        }

        /// <summary>
        /// 建立新學習圈的固定角色 - 老師 + 學生
        /// </summary>
        /// <param name="learningId">學習圈代碼</param>
        /// <param name="memberId">管理者代碼</param>
        /// <returns></returns>
        public string[] AddNewLearningRole(int learningId, int memberId)
        {
            var db = _uow.DbContext;
            string[] roleIdArray = new string[2];
            //新增老師角色
            var entityRole = new LearningRole
            {
                IsAdminRole = true,
                LearningId = learningId,
                Name = "老師",
                IsFixed = false,
                Created = Infrastructure.Property.TimeData.Create(DateTime.UtcNow),
                CreateUser = memberId,
                Enable = true,
                UpdateUser = null,
                DeleteUser = null,
                Updated = Infrastructure.Property.TimeData.Create(null),
                Deleted = Infrastructure.Property.TimeData.Create(null),
                ExternalRid = 1
            };
            db.LearningRole.Add(entityRole);
            db.SaveChanges();
            roleIdArray[0] = entityRole.Id.ToString();
            //新增學生角色
            entityRole.Name = "學生";
            entityRole.IsAdminRole = false;
            db.LearningRole.Add(entityRole);
            db.SaveChanges();
            roleIdArray[1] = entityRole.Id.ToString();
            return roleIdArray;
        }


        /// <summary>
        /// 查看角色是否為管理者
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool CheckRoleIsAdmin(int roleId)
        {
            var db = _uow.DbContext;
            var info = db.LearningRole.Find(roleId);
            return info.IsAdminRole;
        }

        /// <summary>
        /// 設定角色是否為管理者
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public Infrastructure.Entity.LearningRole SetRoleIsAdmin(int updateMemberid, int roleId, bool isAdmin)
        {
            var db = _uow.DbContext;
            var info = db.LearningRole.Find(roleId);
            if (info != null)
            {
                info.IsAdminRole = isAdmin;
                info.Updated = Infrastructure.Property.TimeData.Create(DateTime.UtcNow);
                info.UpdateUser = updateMemberid;
                db.SaveChanges();
            }
            return info;
        }
        
    }
}

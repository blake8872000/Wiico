using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Repository;
using WiicoApi.Service.ActivityModule;
using WiicoApi.Service.SignalRService;

namespace WiicoApi.Service.CommenService
{
    public class AuthService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly ModuleService moduleService = new ModuleService();
        private readonly TokenService tokenService = new TokenService();
        private readonly CacheService cacheService = new CacheService();

        public AuthService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得被點名成員
        /// </summary>
        /// <param name="learningId"></param>
        /// <param name="functionKey"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ValueObject.AuthMember> GeFunctionMemberListByLinQ(int learningId, string functionKey)
        {
            var db = _uow.DbContext;
            var response = from m in db.Members
                           join cmr in db.CircleMemberRoleplay on m.Id equals cmr.MemberId
                           join lr in db.LearningRole on cmr.RoleId equals lr.Id
                           join la in db.LearningAuth on lr.Id equals la.LearningRoleId
                           join mf in db.ModuleFunction on la.FunctionId equals mf.Id
                           join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                           where lc.Id == learningId && mf.OutSideKey == functionKey && la.Enable == true && cmr.Enable == true && lr.Enable == true
                           select new Infrastructure.ValueObject.AuthMember
                           {
                               Account = m.Account,
                               AccountId = m.Id,
                               AccountName = m.Name,
                               Picture = m.Photo
                           };

            return response.OrderBy(t => t.Account).ToList();
        }

        /// <summary>
        /// 取得被點名成員
        /// </summary>
        /// <param name="learningId"></param>
        /// <param name="functionKey"></param>
        /// <returns></returns>
        //public IEnumerable<Infrastructure.ValueObject.AuthMember> GeFunctionMemberList(int learningId, string functionKey)
        //{
  
        //    return _uow.IThinkVmRepo.GeFunctionMemberList(learningId, functionKey);
        //}

        /// <summary>
        /// 取得使用者在學習圈內的權限
        /// </summary>
        /// <param name="roleInfo"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.BusinessObject.AuthModuleInfo> LearningRoleAuth(List<LearningRole> roleInfo, int learningId)
        {
            List<Infrastructure.BusinessObject.AuthModuleInfo> result = new List<Infrastructure.BusinessObject.AuthModuleInfo>();
            var db = _uow.DbContext;
            foreach (var _itemSql in roleInfo)
            {
                //取得權限相關資訊
                var query = from la in db.LearningAuth.Where(t => t.LearningRoleId.Equals(_itemSql.Id) && t.Enable.Equals(true))
                            join cmr in db.CircleMemberRoleplay on la.LearningRoleId equals cmr.RoleId

                            join lr in db.LearningRole.Where(t => t.LearningId.Equals(learningId)) on cmr.RoleId equals lr.Id
                            join mf in db.ModuleFunction on la.FunctionId equals mf.Id
                            join m in db.Modules on mf.ModulesId equals m.Id
                            group m by m.Id into g
                            select new
                            {
                                ModulesId = g.Key
                            };

                foreach (var _item in query)
                {
                    //權限模組資訊
                    var resAuthModuleInfo = new Infrastructure.BusinessObject.AuthModuleInfo();
                    //權限功能資訊list
                    var moduleFunctionList = moduleService.ModuleFunctions(_item.ModulesId);

                    resAuthModuleInfo.ModuleId = moduleFunctionList.ToList()[0].ModuleId;
                    resAuthModuleInfo.ModuleName = moduleFunctionList.ToList()[0].ModuleName;
                    resAuthModuleInfo.Functions = moduleFunctionList.ToList()[0].Functions.ToArray();

                    result.Add(resAuthModuleInfo);
                }

            }
            return result;
        }

        /// <summary>
        /// 取得某個學習圈的角色權限
        /// </summary>
        /// <param name="learningId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.RoleModuleFunction> GetMemberRoleLearningAuth(int learningId)
        {
            var db = _uow.DbContext;
            var query = new List<Infrastructure.ViewModel.RoleModuleFunction>();

            //取得某個學習圈內所有的功能
            var sqlCol = from mf in db.ModuleFunction
                         join la in db.LearningAuth on mf.Id equals la.FunctionId
                         join cr in db.CircleMemberRoleplay on la.LearningRoleId equals cr.RoleId
                         join lr in db.LearningRole on cr.RoleId equals lr.Id

                         where lr.LearningId.Equals(learningId) && la.Enable.Equals(true) && cr.Enable.Equals(true) && lr.Enable.Equals(true)
                         group mf.Id by mf into r
                         orderby r.Key.Id
                         select r;
            //取得某個學習圈內所有角色
            var sqlRole = from lr in db.LearningRole
                          where lr.LearningId.Equals(learningId) && lr.Enable.Equals(true)
                          select lr;

            //判斷角色功能權限
            var sqlRoleAuth = from lr in db.LearningRole
                              join cmr in db.CircleMemberRoleplay on lr.Id equals cmr.RoleId

                              join la in db.LearningAuth on cmr.RoleId equals la.LearningRoleId
                              join mf in db.ModuleFunction on la.FunctionId equals mf.Id
                              where lr.LearningId.Equals(learningId) && la.Enable.Equals(true) && lr.Enable.Equals(true)
                              select new
                              {
                                  RoleId = lr.Id,
                                  RoleName = lr.Name,
                                  FunctionId = mf.Id,
                                  FunctionName = mf.Name
                              };

            //取得角色清單
            foreach (var _item in sqlRole)
            {
                var resRoleFunction = new Infrastructure.ViewModel.RoleModuleFunction();

                resRoleFunction.RoleId = _item.Id;
                resRoleFunction.RoleName = _item.Name;
                resRoleFunction.FunctionAuth = new List<Infrastructure.BusinessObject.ModuleFunctions>();
                //判斷角色是否有某個功能權限
                foreach (var _itemFunction in sqlCol)
                {
                    var resFunctionAuth = new Infrastructure.BusinessObject.ModuleFunctions();
                    //判斷該角色是否有某個功能權限
                    var sqlSubRole = sqlRoleAuth.Where(t => t.RoleId.Equals(_item.Id) && t.FunctionId.Equals(_itemFunction.Key.Id));
                    resFunctionAuth.Id = _itemFunction.Key.Id;
                    resFunctionAuth.Name = _itemFunction.Key.Name;
                    if (sqlSubRole.Any())
                        resFunctionAuth.Enable = true;
                    else
                        resFunctionAuth.Enable = false;

                    //將權限結果放進viewModel中
                    resRoleFunction.FunctionAuth.Add(resFunctionAuth);
                }
                query.Add(resRoleFunction);
            }

            return query;

        }

        /// <summary>
        /// 新增學習圈權限
        /// </summary>
        /// <param name="learningRoleId"></param>
        /// <param name="functionId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public LearningAuth AddLearningAuth(int learningRoleId, int functionId, int memberId)
        {
            var db = _uow.DbContext;

            var entity = new LearningAuth
            {
                LearningRoleId = learningRoleId,
                FunctionId = functionId,
                Enable = true,
                Created = Infrastructure.Property.TimeData.Create(DateTime.UtcNow),
                CreateUser = memberId,
                UpdateUser = null,
                DeleteUser = null,
                Updated = Infrastructure.Property.TimeData.Create(null),
                Deleted = Infrastructure.Property.TimeData.Create(null)
            };
            db.LearningAuth.Add(entity);
            db.SaveChanges();
            //取得新的token資料
            var result = db.LearningAuth.Find(entity.Id);
            return result;

        }

        /// <summary>
        /// 新增某個學習圈的所有角色權限
        /// </summary>
        /// <param name="learningCircleId">學習圈編號</param>
        /// <param name="creator">建立者編號</param>
        /// <returns></returns>
        public bool InsertLearningCircleAllRoleAuth(int learningCircleId, int creator)
        {
            try
            {
                var roleService = new LearningRoleService();
                var db = _uow.DbContext;
                //查詢學習圈所有角色
                var roles = roleService.GetLearningCircleRoles(learningCircleId);
                foreach (var role in roles)
                {
                    //查詢該角色欲新增的權限
                    var moduleFunctions = (role.IsAdminRole) ? db.ModuleFunction.Where(t => t.IsAdminAuth == true) : db.ModuleFunction.Where(t => t.IsNormalAuth == true);
                    //開始新增權限
                    foreach (var auth in moduleFunctions)
                    {
                        var entity = new LearningAuth
                        {
                            LearningRoleId = role.Id,
                            FunctionId = auth.Id,
                            Enable = true,
                            Created = Infrastructure.Property.TimeData.Create(DateTime.UtcNow),
                            CreateUser = creator,
                            UpdateUser = null,
                            DeleteUser = null,
                            Updated = Infrastructure.Property.TimeData.Create(null),
                            Deleted = Infrastructure.Property.TimeData.Create(null)
                        };
                        db.LearningAuth.Add(entity);
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 設定角色在學習圈內的權限
        /// </summary>
        /// <param name="learningRoleId">角色關聯代碼</param>
        /// <param name="functionId">功能代碼</param>
        /// <param name="setAuth">調整權限</param>
        /// <param name="memberId">修改人</param>
        /// <returns></returns>
        public LearningAuth SetLearningSectionAuth(int learningRoleId, int functionId, bool setAuth, int memberId)
        {
            var db = _uow.DbContext;
            //判斷是否已設定過權限
            var checkAuth = db.LearningAuth.Where(t => t.FunctionId.Equals(functionId) && t.LearningRoleId.Equals(learningRoleId));
            var entity = new LearningAuth();

            //直接修改權限
            if (checkAuth.Any())
            {
                entity = checkAuth.ToList()[0];
                entity.Enable = setAuth;

                db.SaveChanges();
            }
            //新增權限
            else
            {
                entity = AddLearningAuth(learningRoleId, functionId, memberId);
            }

            //取得新的token資料
            var result = db.LearningAuth.Find(entity.Id);
            return result;
        }


        /// <summary>
        /// 判斷是否為課程老師
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public bool CheckCourseTeacher(int memberId, string circleKey)
        {
            var db = _uow.DbContext;
            var responseData = (from cmr in db.CircleMemberRoleplay
                                join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                                join lr in db.LearningRole on cmr.RoleId equals lr.Id
                                where lc.LearningOuterKey == circleKey && cmr.MemberId == memberId && lr.Level <= 2
                                select cmr).FirstOrDefault();
            if (responseData == null)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 判斷是否為課程管理者
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public bool CheckCourseAdmin(int memberId, string circleKey)
        {
            var db = _uow.DbContext;
            var responseData = (from lcm in db.LearningCircleManager
                                join lc in db.LearningCircle on lcm.LearningCircleId equals lc.Id
                                where lc.LearningOuterKey == circleKey && lcm.MemberId == memberId && lcm.Enable == true
                                select lcm).FirstOrDefault();
            if (responseData == null)
                return false;
            else
                return true;
        }

        public bool CheckDepartmentAdmin(int memberId, int orgId)
        {

            //分類學院管理者
            var isDepartmentAdmin = (from da in _uow.DbContext.DepartmentAdmin
                                     join d in _uow.DbContext.Depts on da.DeptId equals d.Id
                                     where da.MemberId == memberId && d.OrgId == orgId
                                     select da).FirstOrDefault();
            if (isDepartmentAdmin == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 確認課程管理權限
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public CourseManageAuthResponse CheckCourseManageAuth(string token, string circleKey)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;

            if (memberInfo == null)
                return null;
            var learningCircleService = new LearningCircleService();
            var learningInfo = learningCircleService.GetDetailByOuterKey(circleKey.ToLower());
            if (learningInfo == null)
                return null;
            var responseData = new CourseManageAuthResponse()
            {
                CircleAdminSetting = new CircleAdminSettingAuth(),
                CircleInfoSetting = new CircleInfoSettingAuth(),
                CircleMemberSetting = new CircleMemberSettingAuth(),
                CircleRoleSetting = new CircleRoleSettingAuth(),
                CircleScheduleSetting = new CircleScheduleSettingAuth(),
                CircleTimelistSetting = new CircleTimelistSettingAuth()
            };

            //課程管理者
            var isCourseManage = CheckCourseAdmin(memberInfo.Id, circleKey);
            //分類系所管理者
            var isDepartmentAdmin = CheckDepartmentAdmin(memberInfo.Id, memberInfo.OrgId);
            var learningRoleService = new LearningRoleService();
            //查看查詢者在課程裡的角色
            var myRole = learningRoleService.GetMyRole(token, circleKey);

            //如果是匯入的課程
            if (learningInfo.ExternalRid != null)
            {
                responseData = SetCourseManageAuth(
                    //判斷是否為課程管理者
                    (isCourseManage || ((learningInfo.OrgId.Value == memberInfo.OrgId && memberInfo.IsOrgAdmin) || isDepartmentAdmin)),
                    myRole.Level == 1, //判斷是否為老師身分
                    myRole.Level == 2, //判斷是否為助教身分
                    true);//是匯入所以是true
                return responseData;
            }
            //一般課程管理者
            if (isCourseManage || ((learningInfo.OrgId.Value == memberInfo.OrgId && memberInfo.IsOrgAdmin) || isDepartmentAdmin))
            {
                responseData = SetCourseManageAuth(true);
                return responseData;
            }
            if (myRole == null)
                return null;
            switch (myRole.Level)
            {
                //如果是老師
                case 1:
                    responseData = SetCourseManageAuth(false, true);
                    return responseData;
                //如果是助教
                case 2:
                    responseData = SetCourseManageAuth(false, false, true);
                    return responseData;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 檢查成員是否有功能權限
        /// </summary>
        /// <param name="learningId">學習圈Id</param>
        /// <param name="functionKey">功能 OutSideKey</param>
        /// <param name="memberId">成員memberId</param>
        /// <returns></returns>
        public bool CheckFunctionAuth(int learningId, string functionKey, int? memberId)
        {
            var db = _uow.DbContext;
            var checkResult = (from cmr in db.CircleMemberRoleplay
                               join lr in db.LearningRole on cmr.RoleId equals lr.Id
                               join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                               join la in db.LearningAuth on lr.Id equals la.LearningRoleId
                               join mf in db.ModuleFunction on la.FunctionId equals mf.Id
                               where mf.OutSideKey == functionKey && lc.Id == learningId && cmr.MemberId == memberId.Value
                               select la).ToList();
            if (checkResult.Count() <= 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 同時驗證使用者身分 + 學習圈權限
        /// </summary>
        /// <param name="token">使用者token</param>
        /// <param name="circleKey">學習圈key</param>
        /// <param name="functionName">使用方法名稱</param>
        /// <param name="authString">學習圈權限名稱</param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.TokenCircleAuth CheckTokenCircleAuth(int memberId, string circleKey, string functionName, string authString)
        {
            var result = new Infrastructure.BusinessObject.TokenCircleAuth();
            //JsonResponse.Data[0]代表使用者流水號 
            //JsonResponse.Data[1]代表學習圈流水號
            result.Data = new string[2];
            try
            {
                // 是否為合法使用者
                // var memberId = iThink.Web2.Models.Auth.AuthMember.TokenToUserId(token);
                if (memberId > 0)
                {
                    int id = cacheService.GetCircle(circleKey).Id;
                    var hasAuth = CheckFunctionAuth(id, authString, memberId);
                    if (hasAuth)
                    {
                        result.Success = true;
                        result.Message = string.Format("[{0}]身分驗證成功，權限驗證成功!", functionName);
                        result.Data[0] = memberId.ToString();
                        result.Data[1] = id.ToString();
                        return result;
                    }
                    else
                        result.Success = false;
                    result.Message = string.Format("[{0}]身分驗證成功，但您沒有權限!", functionName);
                    result.Data[0] = memberId.ToString();
                    result.Data[1] = id.ToString();
                    return result;
                }
                else
                    result.Success = false;
                result.Message = string.Format("[{0}]身分驗證失敗!請重新登入!!]", functionName);
                return result;
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                result.Success = false;
                result.Message = string.Format("建立活動發生意外!!:[{0}]", msg);
                return result;
            }
        }

        public bool CheckSystemAdmin(int memberId)
        {
            var memberInfo = _uow.MembersRepo.Get(t => t.Id == memberId).FirstOrDefault();
            if (memberInfo == null || memberInfo.RoleName != "1")
                return false;
            return true;
        }

        private CourseManageAuthResponse SetCourseManageAuth(bool isCourseManage, bool? isteacher = false, bool? isSupport = false, bool? isExternalRid = false, string circleKey = null)
        {
            var responseData = new CourseManageAuthResponse()
            {
                CircleAdminSetting = new CircleAdminSettingAuth(),
                CircleInfoSetting = new CircleInfoSettingAuth(),
                CircleMemberSetting = new CircleMemberSettingAuth(),
                CircleRoleSetting = new CircleRoleSettingAuth(),
                CircleScheduleSetting = new CircleScheduleSettingAuth(),
                CircleTimelistSetting = new CircleTimelistSettingAuth()
            };
            //同步過來的資料
            if (isExternalRid.HasValue && isExternalRid.Value)
            {
                var learningCircleInfo = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == circleKey.ToLower());
                if (learningCircleInfo == null)
                    return null;
                var externalDatas = from er in _uow.DbContext.ExtResources
                                    join ert in _uow.DbContext.ExtResTypes on er.ExternalResTypeId equals ert.Id
                                    where er.Enable == true && er.OrgId == learningCircleInfo.OrgId
                                    select new { er, ert };

                responseData.CircleAdminSetting.Admin = isCourseManage ? true : false;
                responseData.CircleAdminSetting.DeleteCircleAdmin = isCourseManage ? true : false;

                responseData.CircleInfoSetting.Admin = false;

                responseData.CircleMemberSetting.Admin = (isCourseManage || isteacher.Value) ? true : false;
                responseData.CircleMemberSetting.AddCircleMember = (isCourseManage || isteacher.Value) ? true : false;
                responseData.CircleMemberSetting.DeleteCircleMember = false;
                responseData.CircleMemberSetting.EditLevelOne = false;

                responseData.CircleRoleSetting.AddCircleRole = (isCourseManage || isteacher.Value) ? true : false;
                responseData.CircleRoleSetting.Admin = (isCourseManage || isteacher.Value) ? true : false;
                responseData.CircleRoleSetting.DeleteCircleRole = (isCourseManage || isteacher.Value) ? true : false;
                responseData.CircleRoleSetting.AddLevelOne = (isCourseManage && isteacher.Value == false) ? true : false;

                responseData.CircleScheduleSetting.Admin = (externalDatas.FirstOrDefault(t => t.ert.AsyncTypeCode == "schedule" && t.er.Enable) != null) ? false : true;
                responseData.CircleTimelistSetting.Admin = (externalDatas.FirstOrDefault(t => t.ert.AsyncTypeCode == "syncClass" && t.er.Enable) != null) ? false : true;

                return responseData;
            }
            //管理者權限
            if (isCourseManage)
            {
                responseData.CircleAdminSetting.Admin = true;
                responseData.CircleAdminSetting.DeleteCircleAdmin = true;
                responseData.CircleInfoSetting.Admin = true;
                responseData.CircleMemberSetting.Admin = true;
                responseData.CircleMemberSetting.AddCircleMember = true;
                responseData.CircleMemberSetting.DeleteCircleMember = true;
                responseData.CircleRoleSetting.AddCircleRole = true;
                responseData.CircleRoleSetting.Admin = true;
                responseData.CircleRoleSetting.DeleteCircleRole = true;
                responseData.CircleRoleSetting.AddLevelOne = true;
                responseData.CircleScheduleSetting.Admin = true;
                responseData.CircleTimelistSetting.Admin = true;
                responseData.CircleMemberSetting.EditLevelOne = true;
                return responseData;
            }
            //老師權限
            if (isteacher.HasValue && isteacher.Value)
            {
                responseData.CircleAdminSetting.Admin = false;
                responseData.CircleAdminSetting.DeleteCircleAdmin = false;

                responseData.CircleInfoSetting.Admin = true;

                responseData.CircleMemberSetting.Admin = true;
                responseData.CircleMemberSetting.AddCircleMember = false;
                responseData.CircleMemberSetting.DeleteCircleMember = false;
                responseData.CircleMemberSetting.EditLevelOne = false;

                responseData.CircleRoleSetting.AddCircleRole = true;
                responseData.CircleRoleSetting.Admin = true;
                responseData.CircleRoleSetting.DeleteCircleRole = true;
                responseData.CircleRoleSetting.AddLevelOne = false;

                responseData.CircleScheduleSetting.Admin = true;
                responseData.CircleTimelistSetting.Admin = true;
                return responseData;
            }
            //助教權限
            if (isSupport.HasValue && isSupport.Value)
            {
                responseData.CircleAdminSetting.Admin = false;
                responseData.CircleAdminSetting.DeleteCircleAdmin = false;

                responseData.CircleInfoSetting.Admin = true;

                responseData.CircleMemberSetting.Admin = false;
                responseData.CircleMemberSetting.AddCircleMember = false;
                responseData.CircleMemberSetting.DeleteCircleMember = false;
                responseData.CircleMemberSetting.EditLevelOne = false;

                responseData.CircleRoleSetting.AddCircleRole = false;
                responseData.CircleRoleSetting.Admin = false;
                responseData.CircleRoleSetting.DeleteCircleRole = false;
                responseData.CircleRoleSetting.AddLevelOne = false;

                responseData.CircleScheduleSetting.Admin = true;
                responseData.CircleTimelistSetting.Admin = true;
                return responseData;
            }
            return responseData;
        }
    }
}
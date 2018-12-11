using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class OrganizationService
    {
        private readonly GenericUnitOfWork _uow;

        public OrganizationService(GenericUnitOfWork uow) {
            _uow = uow;
        }
        public OrganizationService()
        {
            _uow = new GenericUnitOfWork();
        }

        public Organization GetDetailByOrgId(int orgId, string token)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return null;
            var orgInfo = _uow.DbContext.Organizations.Find(orgId);
            if (orgInfo == null || memberInfo.OrgId != orgInfo.Id)
                return null;
            return orgInfo;
        }
        public Organization GetDetailByOrgCode(string orgCode, string token)
        {
            var orgInfo = _uow.DbContext.Organizations.FirstOrDefault(t => t.OrgCode.ToLower() == orgCode.ToLower());
            if (orgInfo == null)
                return null;
            return GetDetailByOrgId(orgInfo.Id, token);
        }

        /// <summary>
        /// 判斷組織是否可以讓外面的人註冊成員
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public bool CheckCanRegister(string orgCode)
        {
            var db = _uow.DbContext;
            var responseData = db.Organizations.FirstOrDefault(t => t.OrgCode.ToLower() == orgCode.ToLower());
            if (responseData == null)
                return false;
            return responseData.IsOrgRegister;
        }

        /// <summary>
        /// 確認帳號有多少個組織
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.Login.AccountConfirmOrganization> AccountConfirmOrganizations(string account)
        {
            var db = _uow.DbContext;
            var dbQuery = (from og in db.Organizations
                           join m in db.Members on og.Id equals m.OrgId
                           where m.Account == account
                           select new Infrastructure.ViewModel.Login.AccountConfirmOrganization
                           {
                               Id = og.Id,
                               OrgCode = og.OrgCode,
                               OrgName = og.Name
                           }).ToList();
            if (dbQuery.FirstOrDefault() == null)
                return null;
            return dbQuery;
        }

        /// <summary>
        /// 查詢組織資訊
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public IEnumerable<Organization> GetOrganization(string token, string searchName, int? orgid = null, int? page = 1, int? row = null)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return null;
            if (memberInfo.RoleName == null || memberInfo.RoleName == string.Empty)
                return null;
            var memberRoleName = Convert.ToInt32(memberInfo.RoleName);

            var accountRoleInfo = _uow.SystemRoleRepo.GetFirst(t => t.Id == memberRoleName);
            if (accountRoleInfo.IsSystemManager)
            {
                var responseData = orgid.HasValue ? _uow.OrganizationRepo.Get(t => t.Id == orgid.Value) : _uow.OrganizationRepo.Get(t => t.Visibility == true);
                if (orgid == null && searchName != null && searchName != string.Empty)
                    //有搜尋字串
                    responseData = responseData.Where(t => t.Name.StartsWith(searchName) || t.OrgCode.StartsWith(searchName));

                return responseData.ToList();
            }
            else
                return null;
        }

        /// <summary>
        /// 建立組織資訊
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public Organization CreateOrganization(OrganizationPostRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token.ToString()).Result;
            if (checkToken == null)
                return null;
            var entity = new Organization()
            {
                Created = TimeData.Create(DateTime.UtcNow),
                CreateUser = checkToken.MemberId,
                Deleted = TimeData.Create(null),
                Updated = TimeData.Create(null),
                Name = requestData.Name,
                OrgCode = requestData.OrgCode,
                Visibility = true
            };
            if (requestData.ApiKey != string.Empty && requestData.ApiKey != null)
                entity.APIKey = requestData.ApiKey;

            if (requestData.SemesterLength.HasValue)
                entity.SemesterLength = requestData.SemesterLength;
            else
                entity.SemesterLength = 1;
            try
            {
                _uow.OrganizationRepo.Insert(entity);
                _uow.SaveChanges();
                //     CreateSystemRole(checkToken.MemberId, entity.Id, "一般使用者", "GeneralRole", false, false);
                //   CreateSystemRole(checkToken.MemberId, entity.Id, "組織管理者", "OrganizationRole", true, false);
                return entity;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// 編輯組織
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public Organization UpdateOrganization(OrganizationPutRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token.ToString()).Result;
            if (checkToken == null)
                return null;
            var organizationInfo = _uow.OrganizationRepo.Get(t => t.Id == requestData.Id).FirstOrDefault();
            if (organizationInfo == null)
                return null;
            if (requestData.Name != null && requestData.Name != string.Empty)
                organizationInfo.Name = requestData.Name;
            if (requestData.ApiKey != null && requestData.ApiKey != string.Empty)
                organizationInfo.APIKey = requestData.ApiKey;
            if (requestData.SemesterLength.HasValue)
                organizationInfo.SemesterLength = requestData.SemesterLength.Value;



            organizationInfo.Updated = TimeData.Create(DateTime.UtcNow);
            organizationInfo.UpdateUser = checkToken.MemberId;
            try
            {

                _uow.SaveChanges();
                return organizationInfo;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        /// <summary>
        /// 刪除組織
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool DeleteOrganization(OrganizationDeleteRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token.ToString()).Result;
            if (checkToken == null)
                return false;
            var organizationInfo = _uow.OrganizationRepo.Get(t => t.Id == requestData.Id).FirstOrDefault();
            if (organizationInfo == null)
                return false;
            try
            {
                _uow.OrganizationRepo.Delete(organizationInfo);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        /// <summary>
        /// 建立系統角色
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="orgId"></param>
        /// <param name="roleName"></param>
        /// <param name="roleCode"></param>
        /// <param name="isOrgAdmin"></param>
        /// <param name="isSystemAdmin"></param>
        /// <returns></returns>
        public SystemRole CreateSystemRole(int creator, int orgId, string roleName, string roleCode, bool isOrgAdmin, bool isSystemAdmin)
        {
            var entity = new SystemRole()
            {
                Created = TimeData.Create(DateTime.UtcNow),
                CreateUser = creator,
                Deleted = TimeData.Create(null),
                Updated = TimeData.Create(null),
                Name = roleName,
                RoleType = roleCode,
                IsSystemManager = isSystemAdmin,
                Enable = true
            };
            _uow.SystemRoleRepo.Insert(entity);
            _uow.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 取得角色列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IEnumerable<SystemRole> GetSystemRoles()
        {
            var datas = _uow.SystemRoleRepo.GetList();
            return datas.ToList();
        }
    }
}

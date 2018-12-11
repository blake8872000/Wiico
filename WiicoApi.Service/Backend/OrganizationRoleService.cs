using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class OrganizationRoleService
    {
        private readonly GenericUnitOfWork _uow;


        public OrganizationRoleService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }

        public OrganizationRoleService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<OrganizationRole> GetListByRequest(OrganizationRoleGetRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token).Result;
            if (checkToken == null)
                return null;

            var responseData = _uow.OrganizationRoleRepo.GetListByRequest(requestData);
            if (responseData == null)
                return null;

            return responseData;
        }

        /// <summary>
        /// 處理多筆資料 - 新增 刪除 修改
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<OrganizationRole> PostByRequest(OrganizationRolePostRequest requestData)
        {
            var db = _uow.DbContext;
            var organizationInfo = _uow.OrganizationRepo.Get(t => t.OrgCode == requestData.OrgCode).FirstOrDefault();
            if (organizationInfo == null)
                return null;

            var dbDatas = GetListByRequest(requestData);
            if (dbDatas == null)
                return null;
            var insertDatas = requestData.OrgRoles.Where(t => t.Id <= 0).ToList();
            foreach (var insertData in insertDatas)
            {
                insertData.OrgId = organizationInfo.Id;
                db.OrganizationRole.Add(insertData);
            }
            var updateDatas = requestData.OrgRoles.Where(t => t.Id > 0).ToList();
            foreach (var updateData in updateDatas)
            {
                var changeData = db.OrganizationRole.Find(updateData.Id);
                if (changeData == null)
                    continue;
                changeData.RoleCode = updateData.RoleCode;
                changeData.Name = updateData.Name;
                changeData.IsAdmin = updateData.IsAdmin;
                if (updateData.Level > 0)
                    changeData.Level = updateData.Level;
            }
            var unionDatas = updateDatas.Select(t => t.Id).Union(dbDatas.Select(t => t.Id));
            var deleteDatas = unionDatas.Except(updateDatas.Select(t => t.Id));
            foreach (var deleteData in deleteDatas)
            {
                var removeData = db.OrganizationRole.Find(deleteData);
                if (removeData == null)
                    continue;
                db.OrganizationRole.Remove(removeData);
            }
            try
            {
                _uow.SaveChanges();
                db.SaveChanges();
                var responseData = GetListByRequest(requestData);
                return responseData;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }
    }
}

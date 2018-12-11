using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class LearningTemplateRoleService
    {
        private readonly GenericUnitOfWork _uow;
        public LearningTemplateRoleService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }

        public LearningTemplateRoleService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<LearningTemplateRoles> GetListByRequest(BackendBaseRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token);
            if (checkToken == null)
                return null;
            var learningTemplateRoleService = new LearningTemplateRoleService();
            var responseData = _uow.LearningTemplateRoleRepo.GetListByRequest(requestData);
            return responseData;
        }
        /// <summary>
        /// 資料處理
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<LearningTemplateRoles> DataProxy(LearningTemplateRolePostRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token);
            if (checkToken == null)
                return null;
            var organizationInfo = _uow.OrganizationRepo.Get(t => t.OrgCode == requestData.OrgCode).FirstOrDefault();
            if (organizationInfo == null)
                return null;
            var db = _uow.DbContext;
            try
            {
                var insertDatas = requestData.CourseRoles.Where(t => t.Id <= 0).ToList();
                //整理欲新增的資料
                foreach (var insertData in insertDatas)
                {
                    insertData.OrgId = organizationInfo.Id;
                    db.LearningTemplateRoles.Add(insertData);
                }
                //欲編輯資料
                var updateDatas = requestData.CourseRoles.Where(t => t.Id > 0).ToList();
                //DB目前資料
                var dbDatas = (from ltr in db.LearningTemplateRoles
                               join og in db.Organizations on ltr.OrgId equals og.Id
                               where og.OrgCode == requestData.OrgCode
                               select ltr).ToList();
                //更新資料
                foreach (var dbData in dbDatas)
                {
                    var updateData = updateDatas.FirstOrDefault(t => t.Id == dbData.Id);
                    if (updateData == null)
                        continue;
                    dbData.Name = updateData.Name;
                    dbData.RoleKey = updateData.RoleKey;
                    dbData.Level = updateData.Level;
                }
                //聯集編號
                var unionDatas = dbDatas.Select(t => t.Id).Union(updateDatas.Select(t => t.Id));
                //找出欲刪除的編號
                var deleteDatas = unionDatas.Except(updateDatas.Select(t => t.Id));
                foreach (var deleteData in deleteDatas)
                {
                    var removeData = db.LearningTemplateRoles.Find(deleteData);
                    if (removeData == null)
                        continue;
                    db.LearningTemplateRoles.Remove(removeData);
                }

                db.SaveChanges();
                return GetListByRequest(requestData);
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }
    }
}

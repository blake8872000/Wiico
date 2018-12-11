using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{

    public class ExternalResourceService
    {
        private GenericUnitOfWork _uow;
        public ExternalResourceService(GenericUnitOfWork uow)
        {
            _uow =uow;
        }
        public ExternalResourceService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<ExternalResource> GetListByRequest(BackendBaseRequest requestData)
        {
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token).Result;
            if (checkToken == null)
                return null;
            var responseData = _uow.ExternalResourceRepo.GetListByRequest(requestData);
            if (responseData == null)
                return null;

            return responseData;
        }
        /// <summary>
        /// 資料處理
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<ExternalResource> DataProxy(ExternalResourcePostRequest requestData)
        {

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token).Result;
            if (checkToken == null)
                return null;
            var organizationInfo = _uow.OrganizationRepo.Get(t => t.OrgCode == requestData.OrgCode).FirstOrDefault();
            if (organizationInfo == null)
                return null;
            var db = _uow.DbContext;

            try
            {
                var insertDatas = requestData.Apis.Where(t => t.Id <= 0).ToList();
                //整理欲新增的資料
                foreach (var insertData in insertDatas)
                {
                    insertData.LastModifyUtc = null;
                    insertData.Status = false;
                    insertData.Created = TimeData.Create(DateTime.UtcNow);
                    insertData.Deleted = TimeData.Create(null);
                    insertData.Updated = TimeData.Create(null);
                    insertData.CreateUser = checkToken.MemberId;
                    insertData.Enable = true;
                    insertData.OrgId = organizationInfo.Id;
                    db.ExtResources.Add(insertData);
                }
                //欲編輯資料
                var updateDatas = requestData.Apis.Where(t => t.Id > 0).ToList();
                //DB目前資料
                var dbDatas = (from er in db.ExtResources
                               join og in db.Organizations on er.OrgId.Value equals og.Id
                               where og.OrgCode == requestData.OrgCode
                               select er).ToList();
                //更新資料
                foreach (var dbData in dbDatas)
                {
                    var updateData = updateDatas.FirstOrDefault(t => t.Id == dbData.Id);
                    if (updateData == null)
                        continue;
                    dbData.Updated = TimeData.Create(DateTime.UtcNow);
                    dbData.UpdateUser = checkToken.MemberId;
                    dbData.Name = updateData.Name;
                    dbData.Uri = updateData.Uri;
                    dbData.ExternalResTypeId = updateData.ExternalResTypeId;
                    dbData.Enable = updateData.Enable;
                }
                //聯集編號
                var unionDatas = dbDatas.Select(t => t.Id).Union(updateDatas.Select(t => t.Id));
                //找出欲刪除的編號
                var deleteDatas = unionDatas.Except(updateDatas.Select(t => t.Id));
                foreach (var deleteData in deleteDatas)
                {
                    var removeData = db.ExtResources.Find(deleteData);
                    if (removeData == null)
                        continue;
                    db.ExtResources.Remove(removeData);
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
        /// <summary>
        /// 建立資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ExternalResource CreateData(ExternalResourcePostRequest requestData) { return null; }

    }
}


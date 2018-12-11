using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Repository;

namespace WiicoApi.Service.Backend
{
    public class BackendService
    {
        private readonly GenericUnitOfWork _uow;

        public BackendService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得組織管理者頁面資訊
        /// </summary>
        /// <param name="orgId">組織編號</param>
        /// <returns></returns>
        public OrgManageViewModel GetOrgManagePageInfo(int orgId)
        {
            var db = _uow.DbContext;
            var result = new OrgManageViewModel();

            //組織
            result.OrgInfo = db.Organizations.Find(orgId);

            var dbData = from o in db.Organizations
                         join m in db.Members on o.Id equals m.OrgId
                         where o.Id == orgId && m.Enable == true && m.Visibility == true
                         select m;

            //組織人員
            result.Members = dbData.ToList();

            //組織管理員
            result.OrgManagers = dbData.Where(t => t.IsOrgAdmin == true).ToList();
            return result;
        }
        /// <summary>
        /// 取得非教務匯入的成員列表 - 後臺管理專用
        /// </summary>
        /// <param name="orgId">組織編號</param>
        /// <returns></returns>
        public OrganizationMemberViewModel GetOrganizationMembers(int orgId)
        {
            var db = _uow.DbContext;
            var result = new OrganizationMemberViewModel();
            //組織
            result.OrgInfo = db.Organizations.Find(orgId);

            //取得非教務匯入的成員列表
            var dbData = from o in db.Organizations
                         join m in db.Members on o.Id equals m.OrgId
                         where o.Id == orgId && m.Enable == true && m.Visibility == true && m.ExternalRid == null
                         select m;
            result.MemberInfo = dbData.Take(500).ToList();
            return result;
        }

        /// <summary>
        /// 取得註冊完成的組織列表 - 後臺管理專用
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Organization> GetOrganizationList()
        {
            var db = _uow.DbContext;
            var dbData = db.Organizations.Where(t => t.Visibility == true);
            return dbData;
        }

        /// <summary>
        /// 取得學校資訊
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Organization GetOrgizationInfo(int orgId)
        {
            var db = _uow.DbContext;
            return db.Organizations.Find(orgId);
        }

        /// <summary>
        /// 取得擴充欄位資訊
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IEnumerable<ExtensionColumn> GetExtensionColumns(int orgId)
        {
            var result = _uow.DbContext.ExtensionColumn.Where(t => t.OrgId == orgId && t.Enable == true).OrderBy(t => t.Sort).ToList();
            return result;
        }

    }
}

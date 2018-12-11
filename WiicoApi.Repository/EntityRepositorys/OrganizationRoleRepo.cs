using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class OrganizationRoleRepo : GenericEntityRepository<OrganizationRole,WiicoDB>
    {
        public OrganizationRoleRepo(WiicoDB context) : base(context)
        {

        }

        /// <summary>
        /// 根據Request取得列表
        /// </summary>
        /// <returns></returns>
        public List<OrganizationRole> GetListByRequest(OrganizationRoleGetRequest requestData) {
            var sqlQuery = from or in _context.OrganizationRole
                           join o in _context.Organizations on or.OrgId equals o.Id
                           where o.OrgCode == requestData.OrgCode
                           select or;
            if (requestData.Search != null && requestData.Search != string.Empty) {
                var dataId = 0;
                Int32.TryParse(requestData.Search, out dataId);
                sqlQuery = dataId > 0 ? sqlQuery.Where(t => t.Id == dataId) : sqlQuery.Where(t => t.Name.StartsWith(requestData.Search) || t.RoleCode.StartsWith(requestData.Search));
            }
            var response = sqlQuery.ToList();
            if (response.FirstOrDefault() == null)
                return null;
            return response;
        }
    }
}


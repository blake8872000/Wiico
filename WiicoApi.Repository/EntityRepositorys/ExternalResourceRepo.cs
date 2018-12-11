using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class ExternalResourceRepo : GenericEntityRepository<ExternalResource,WiicoDB>
    {
        public ExternalResourceRepo(WiicoDB _context) : base(_context)
        {

        }
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<ExternalResource> GetListByRequest(BackendBaseRequest requestData) {
            var dbQuery = (requestData.OrgCode != null && requestData.OrgCode != string.Empty) ?
                            (from er in _context.ExtResources
                             join og in _context.Organizations on er.OrgId equals og.Id
                             where og.OrgCode == requestData.OrgCode
                             select er).ToList() :
                           ((_context.ExtResources.ToList()));
             if(dbQuery.FirstOrDefault()==null)
                return null;

            return dbQuery;
        }
    }
}

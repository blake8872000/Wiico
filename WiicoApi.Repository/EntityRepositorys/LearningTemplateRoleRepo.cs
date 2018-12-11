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
    public class LearningTemplateRoleRepo : GenericEntityRepository<LearningTemplateRoles,WiicoDB>
    {
        public LearningTemplateRoleRepo(WiicoDB _context) : base(_context)
        {
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<LearningTemplateRoles> GetListByRequest(BackendBaseRequest requestData) {
            var dbQuery = (from ltr in _context.LearningTemplateRoles
                           join og in _context.Organizations on ltr.OrgId equals og.Id
                           where og.OrgCode == requestData.OrgCode
                           select ltr).ToList();
            if (dbQuery.FirstOrDefault() == null)
                return null;
            return dbQuery;
        }
    }
}
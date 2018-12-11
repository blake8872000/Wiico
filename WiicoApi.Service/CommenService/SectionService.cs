using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class SectionService
    {
        private readonly GenericUnitOfWork _uow;

        public SectionService()
        {
            _uow = new GenericUnitOfWork();
        }
        public Section GetOrgNowSeme(int orgId)
        {
            var db = _uow.DbContext;
            var data = db.Sections.FirstOrDefault(t => t.OrgId == orgId && t.IsNowSeme == true);
            return data;
        }
    }
}

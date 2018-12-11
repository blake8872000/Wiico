using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class DeptRepo : GenericEntityRepository<Dept,WiicoDB>
    {
        public DeptRepo(WiicoDB _context) : base(_context)
        {

        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<Dept> GetList(int orgId,string searchName = null)
        {
            var list = from d in _context.Depts
                        join o in _context.Organizations on d.OrgId equals o.Id
                        where o.Id==orgId
                        select d;
            var response = new List<Dept>();
            if (searchName != null)
                list.Where(t => t.Name.StartsWith(searchName) || t.DeptCode.StartsWith(searchName));


            if (list.FirstOrDefault() != null) {
                response = list.ToList();
                return response;
            }
            else
                return null;
        }
    }
}

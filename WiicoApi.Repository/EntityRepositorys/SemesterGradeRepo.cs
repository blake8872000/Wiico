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
    public class SemesterGradeRepo : GenericEntityRepository<SemesterGrade,WiicoDB>
    {
        public SemesterGradeRepo (WiicoDB _context) : base(_context)
        {
        }
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public List<SemesterGrade> GetListByRequest(BackendBaseRequest requestData) {
            var dbQuery = (from sg in _context.SemesterGrade
                           join og in _context.Organizations on sg.OrgId equals og.Id
                           where og.OrgCode == requestData.OrgCode
                           select sg).ToList();
            if (dbQuery.FirstOrDefault() == null)
                return null;
            return dbQuery;
        }
    }
}

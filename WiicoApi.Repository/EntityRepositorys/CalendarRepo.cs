using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
namespace WiicoApi.Repository.EntityRepositorys
{
    public class CalendarRepo : GenericEntityRepository<Calendar,WiicoDB>
    {

        public CalendarRepo(WiicoDB _context) : base(_context)
        {
        }

    }
}

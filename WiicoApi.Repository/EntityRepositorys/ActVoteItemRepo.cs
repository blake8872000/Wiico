using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
namespace WiicoApi.Repository.EntityRepositorys
{
    public class ActVoteItemRepo : GenericEntityRepository<ActVoteItem,WiicoDB>
    {
        public ActVoteItemRepo(WiicoDB _context) : base(_context)
        {

        }
        /// <summary>
        /// 建立投票項目
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public IEnumerable<ActVoteItem> CreateItems(List<ActVoteItem> items) {
            _context.ActVoteitem.AddRange(items);
            _context.SaveChanges();
            return items;
        }

    }
}

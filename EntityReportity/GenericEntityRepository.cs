using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityRepository
{
    public class GenericEntityRepository<TEntity,DC> : IEntityRepository<TEntity>
           where TEntity : class where DC : DbContext
    {
        protected DC _context = null;

        private DbSet<TEntity> _Objectset;
        private DbSet<TEntity> ObjectSet
        {
            get
            {
                if (_Objectset == null)
                    _Objectset = _context.Set<TEntity>();
                return _Objectset;
            }
        }

        public GenericEntityRepository(DC context)
        {
            this._context =  context ==null ? throw new ArgumentNullException("context") :  context;
        }

        public void Insert(TEntity entity)
        {
            ObjectSet.Add(entity);
        }

        /// <summary>
        /// 多個新增
        /// </summary>
        /// <param name="entity"></param>
        public void MultipleInsert(List<TEntity> entity)
        {
            foreach (var _entity in entity)
            {
                ObjectSet.Add(_entity);
            }
        }

        public void Delete(TEntity entity)
        {
            ObjectSet.Remove(entity);
        }

        public IEnumerable<TEntity> GetList()
        {
            return ObjectSet;
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            return ObjectSet.Where(filter);
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> filter)
        {
            return ObjectSet.FirstOrDefault(filter);
        }
    }
}

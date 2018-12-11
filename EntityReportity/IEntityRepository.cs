using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityRepository
{
    public interface IEntityRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 新增一個entity
        /// </summary>
        /// <param name="entity"></param>
        void Insert(TEntity entity);
          
        /// <summary>
        /// 新增多個entity
        /// </summary>
        /// <param name="entity"></param>
        void MultipleInsert(List<TEntity> entity);

        /// <summary>
        /// 刪除單一entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 取得所有列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetList();

        /// <summary>
        /// 下條件取得列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 取得第一筆 entity
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        TEntity GetFirst(Expression<Func<TEntity, bool>> filter);
    }
}

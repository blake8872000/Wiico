using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityRepository
{
   public  class GenericEntityUnitOfWork<DC> : IDisposable where DC : DbContext
    {
        public DC _dbContext;
     
        public DC DbContext
        {
            get
            {
                return _dbContext;
            }
        }

        public Dictionary<Type, object> entityRepositories = new Dictionary<Type, object>();

        public IEntityRepository<T> EntityRepository<T>() where T : class
        {
            if (entityRepositories.Keys.Contains(typeof(T)) == true)
            {
                return entityRepositories[typeof(T)] as IEntityRepository<T>;
            }
            IEntityRepository<T> repo = new GenericEntityRepository<T, DC>(DbContext);
            entityRepositories.Add(typeof(T), repo);
            return repo;
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using POSTagging.Repository;
using System.Data.Entity;

namespace POSTagging.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        private DbSet<TEntity> Entities;

        public Repository(DbContext context)
        {
            Context = context;
            this.Entities = Context.Set<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TEntity entity)
        {
            lock (this.Entities)
            {
                this.Entities.Add(entity);
                Context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            lock (this.Entities)
            {
                this.Entities.AddRange(entities);
                Context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return this.Entities.Where(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity Get(int id)
        {
            lock (this.Entities)
            {
                return this.Entities.Find(id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll()
        {
            lock (this.Entities) {
                return this.Entities.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(TEntity entity)
        {
            lock (this.Entities)
            {
                this.Entities.Remove(entity);
                Context.SaveChanges();
            }
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            lock (this.Entities)
            {
                this.Entities.RemoveRange(entities);
                Context.SaveChanges();
            }
        }
    }
}

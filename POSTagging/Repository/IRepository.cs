using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace POSTagging.Repository
{
    public interface IRepository<IEntity> where IEntity : class

    {
        IEntity Get(int id);
        IEnumerable<IEntity> GetAll();
        void Add(IEntity entity);
        void AddRange(IEnumerable<IEntity> entities);
        void Remove(IEntity entity);
        void RemoveRange(IEnumerable<IEntity> entities);
    }
}

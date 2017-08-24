using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace POSTagging.Repository
{
    // <summary>
    // Mongo Extras
    public interface IMongoRepository<IEntity> where IEntity : class
	{
		Task<IEnumerable<IEntity>> FindAsync(Expression<Func<IEntity, bool>> predicate);
    }
}

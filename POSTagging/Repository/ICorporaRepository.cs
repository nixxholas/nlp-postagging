using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using POSTagging.Repository;
using POSTagging.Tagging;

namespace VaultixServer.Repository
{
    public interface ICorporaRepository : IRepository<WordTag>
    {
        void DefaultAdd(WordTag entity);
		WordTag Get(string word);
		IEnumerable<WordTag> GetAllByTag(string tag);
        IEnumerable<WordTag> GetAllUndone();
        Task<WordTag> FindOneAsync(Expression<Func<WordTag, bool>> predicate);
        Task EditCorpusTags(WordTag corpus);
    }
}

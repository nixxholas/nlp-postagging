using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using POSTagging.Tagging;
using VaultixServer.Repository;

namespace POSTagging.Repository
{
    public class LocalCorporaRepository : IRepository<WordTag>
    {
        public List<WordTag> _localDb = new List<WordTag>();

        private bool isExists(string _word) {
            foreach(WordTag w in _localDb) {
                if (w.Word.Equals(_word)) {
                    return true;
                }
            }

            return false;
        }

        public void Add(WordTag entity)
        {
            if (!isExists(entity.Word))
            _localDb.Add(entity);
        }

        public void AddRange(IEnumerable<WordTag> entities)
        {
            foreach (WordTag w in entities) {
                if (!isExists(w.Word)) {
                    _localDb.Add(w);
                }
            }
        }

        public void DefaultAdd(WordTag entity)
		{
			_localDb.Add(entity);
        }

        public void EditCorpusTags(WordTag corpus)
		{
			foreach (WordTag w in _localDb)
			{
                if (w.Word.Equals(corpus.Word))
				{
                    _localDb.Remove(w);
                    _localDb.Add(corpus);
				}
			}
        }

        public WordTag Get(string word)
        {
            foreach (WordTag w in _localDb) {
                if (w.Word.Equals(word)) {
                    return w;
                }
            }

            return null;
        }

        public IEnumerable<WordTag> GetAll()
        {
            return _localDb;
        }

        public IEnumerable<WordTag> GetAllByTag(string tag)
        {
            List<WordTag> result = new List<WordTag>();

            foreach (WordTag w in _localDb) {
                // https://stackoverflow.com/questions/2912476/using-c-sharp-to-check-if-string-contains-a-string-in-string-array
                if (w.Tag.Contains(tag)) {
                    result.Add(w);
                }
            }

            return result;
        }

        public void Remove(WordTag entity)
        {
            foreach (WordTag w in _localDb) {
                if (w.Word.Equals(entity.Word)) {
                    _localDb.Remove(w);
                }
            }
        }

        public void RemoveRange(IEnumerable<WordTag> entities)
        {
            foreach (WordTag w in entities) {
                foreach (WordTag wordTag in _localDb) {
                    if (wordTag.Word.Equals(w.Word)) {
                        _localDb.Remove(wordTag);
                    }
                }
            }
        }

        public WordTag Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}

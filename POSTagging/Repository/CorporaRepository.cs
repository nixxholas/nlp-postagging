using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using POSTagging.Data;
using POSTagging.Tagging;
using VaultixServer.Repository;

namespace POSTagging.Repository
{
    public class CorporaRepository : IRepository<WordTag>, IMongoRepository<WordTag>, ICorporaRepository
	{
		private readonly MongoDBContext dbContext = new MongoDBContext();

		public void Add(WordTag entity)
		{
            // Only add if we can't find the word
            if (!dbContext.CorpusData.Find(e => e.Word.Equals(entity.Word)).Any())
            {
                dbContext.CorpusData.InsertOne(entity);
                dbContext.CorpusDataOnline.InsertOneAsync(entity);
            }
		}

		public void ForcedAdd(WordTag entity)
		{
            // Delete the existing words first
            if (dbContext.CorpusData.Find(e => e.Word.Equals(entity.Word)).Any())
            {
				dbContext.CorpusData.DeleteMany(e => e.Word.Equals(entity.Word));
                dbContext.CorpusDataOnline.DeleteManyAsync(e => e.Word.Equals(entity.Word));
            }
			dbContext.CorpusData.InsertOne(entity);
            dbContext.CorpusData.InsertOneAsync(entity);
        }

        public void DefaultAdd(WordTag entity) {
            dbContext.CorpusData.InsertOne(entity);
        }

		public async Task<IEnumerable<WordTag>> FindAsync(Expression<Func<WordTag, bool>> predicate)
		{
            return await dbContext.CorpusData.Find(predicate).ToListAsync();
		}

		public async Task<WordTag> FindOneAsync(Expression<Func<WordTag, bool>> predicate)
		{
			if (dbContext.CorpusData.Find(predicate).Any())
			{
				return await dbContext.CorpusData.Find(predicate).FirstOrDefaultAsync();
            }

            return new WordTag("", null);
		}

		public WordTag Get(int id)
		{
			throw new NotImplementedException();
		}

		public WordTag Get(string word)
		{
            return dbContext.CorpusData.Find(entity => entity.Word.Equals(word)).FirstOrDefault();
		}

		public IEnumerable<WordTag> GetAllByTag(string tag)
		{
            return dbContext.CorpusData.Find(entity => entity.Tag.Contains(tag)).ToList();
		}

        public IEnumerable<WordTag> GetAllUndone() {
            return dbContext.CorpusData.Find(entity => entity.Tag == 
                                             new string[] { "UNDONE" }).ToList();
        }

		public IEnumerable<WordTag> GetAll()
		{
			return dbContext.CorpusData.Find(Builders<WordTag>.Filter.Empty).ToList();
		}

		public void Remove(WordTag entity)
		{
			// Only remove if we can find the word
			if (!dbContext.CorpusData.Find(e => e.Word == entity.Word).Any())
			{
                dbContext.CorpusData.DeleteOne(e => e.Word.Equals(entity.Word));
			}
		}

		public void RemoveRange(IEnumerable<WordTag> entities)
		{
			throw new NotImplementedException();
		}

		public void AddRange(IEnumerable<WordTag> entities)
		{
			throw new NotImplementedException();
		}

        public async Task EditCorpusTags(WordTag corpus)
		{
			// Only add if we can find the word
			if (dbContext.CorpusData.Find(e => e.Word == corpus.Word).Any())
			{
				// We don't want to modify the entire entity
				await dbContext.CorpusData.FindOneAndUpdateAsync(entity => entity.Word.Equals(corpus.Word),
																			   // Using Update.Set
																			   // https://stackoverflow.com/questions/4818964/how-do-you-update-multiple-field-using-update-set-in-mongodb-using-official-c-sh
																			   // You need to use Builders... -> https://stackoverflow.com/questions/41502078/inserting-updating-data-in-monogodb-3-4-with-c-sharp-driver-2-3-in-an-mvc-appl
																			   Builders<WordTag>.Update.Set(entity => entity.Tag, corpus.Tag));
            } else {
                // Since we can't find the word
                corpus.Id = ObjectId.GenerateNewId();
                Add(corpus);
            }
        }
    }
}

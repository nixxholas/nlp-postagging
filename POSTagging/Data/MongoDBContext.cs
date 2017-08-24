using MongoDB.Driver;
using System;
using POSTagging.Tagging;
using System.Collections.Generic;

namespace POSTagging.Data
{
    public class MongoDBContext
    {
        private IMongoDatabase _database { get; }
        private IMongoCollection<WordTag> _corpusDataLocal { get; }

        public MongoDBContext()
        {
            string ConnectionString = "mongodb://testuser:testing@ds058508.mlab.com:58508/testpos"; // default as null. Intended to throw exception if its not there.
            string DatabaseName = "testpos"; // default as null. Intended to throw exception if its not there.
            bool IsSSL = true; // default as null. Intended to throw exception if its not there.


            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                }
                var mongoClient = new MongoClient(settings);
                _database = mongoClient.GetDatabase(DatabaseName);

                _corpusDataLocal = _database.GetCollection<WordTag>("Corpus");
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access to db server.", ex);
            }

            //ConnectionString = null; // clear it off memory faster... dont want it in the stack =\ 
            //DatabaseName = null; // clear it off memory faster... dont want it in the stack =\ 

            // Local method

        }

        /// <summary>
        /// 
        /// </summary>
		public IMongoCollection<WordTag> CorpusData
		{
			get
			{
                return _corpusDataLocal;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public IMongoCollection<WordTag> CorpusDataOnline {
            get {
                return _database.GetCollection<WordTag>("CorpusTagData");
            }
        }
    }
}

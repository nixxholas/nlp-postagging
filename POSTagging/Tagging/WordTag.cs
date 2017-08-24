using System;
using MongoDB.Bson;

namespace POSTagging.Tagging
{
	public class WordTag
	{
		/// <summary>
		/// Identification Column for Mongo Rows
		/// </summary>
		public ObjectId Id { get; set; }

		/// <summary>
		/// The word used for tagging
		/// </summary>

		public string Word { get; private set; }

		/// <summary>
		/// The assigned tag
		/// </summary>

		public string[] Tag { get; private set; }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="id"></param>
		/// <param name="word"></param>
		/// <param name="pTag"></param>

		public WordTag(ObjectId id, string word, string[] pTag)
		{
			Id = id;
			Word = word;
			Tag = pTag;
		}

		/// <summary>
		/// This is when you don't need to give out the id
		/// </summary>
		/// <param name="word"></param>
		/// <param name="pTag"></param>

		public WordTag(string word, string[] pTag)
		{
			Word = word;
			Tag = pTag;
		}
	}
}

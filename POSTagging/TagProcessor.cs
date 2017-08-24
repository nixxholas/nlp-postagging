using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using POSTagging.Repository;
using POSTagging.Tagging;
using VaultixServer.Repository;

namespace POSTagging
{
    public class TagProcessor
    {
        private ICorporaRepository _CorporaRepository { get; set; }
        public TagProcessor() {
            _CorporaRepository = new CorporaRepository();
        }

        public void LoadJson()
        {
            try
            {
                using (StreamReader r = new StreamReader("Tagging/CorporaData.txt"))
                {
                    string line;
                    int item = 0;
                    //string json = r.ReadToEnd();

                    while ((line = r.ReadLine()) != null) {
                        Console.WriteLine("Pushing item " + item);
						var ss = line.Split(' ');
						var word = ss[0];

						_CorporaRepository.DefaultAdd(new WordTag(
							ObjectId.GenerateNewId(), word, ss.Skip(1).ToArray()));
                        item++;
					}
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

		/// <summary>
		/// Assigns parts of speech to each word
		/// </summary>
		/// <param name="words"></param>
		/// <returns></returns>
		public IList<WordTag> Tag(IList<string> words)
		{
			if (words == null || words.Count == 0) return new List<WordTag>();

			int count = words.Count;
			var result = new List<WordTag>(new WordTag[count]);
			var resultTags = new List<string>(new string[count]);

			// https://stackoverflow.com/questions/3639768/parallel-foreach-ordered-execution
			Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async i =>
			{
				var word = RemoveSpecialCharacters(words[i]);
				if (string.IsNullOrEmpty(word))
				{
					resultTags[i] = @"";
				}

				string[] ss = (await _CorporaRepository.FindOneAsync(e => e.Word == word)).Tag;

				// 1/22/2002 mod (from Lisp code): if not in hash, try lower case:
				if (ss == null)
					ss = (await _CorporaRepository.FindOneAsync(e => e.Word == word.ToLower())).Tag;
				if (ss == null && word.Length == 1)
					resultTags[i] = word + "^";
				else if (ss == null)
					resultTags[i] = "NN";
				else
					resultTags[i] = ss[0];
			});

			Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
			{
				string word = words[i];
				string pTag = resultTags[i];

				if (pTag != null)
				{
					//  rule 1: DT, {VBD | VBP} --> DT, NN     
					// If this tag is not the first
					if (i > 0 && string.Equals(resultTags[i - 1], "DT"))
					{
						if (string.Equals(pTag, "VBD") || string.Equals(pTag, "VBP") || string.Equals(pTag, "VB"))
						{
							pTag = "NN";
						}
					}
					// rule 2: convert a noun to a number (CD) if "." appears in the word
					if (pTag.StartsWith("N", StringComparison.CurrentCulture))
					{
						Single s;
						if (word.IndexOf(".", StringComparison.CurrentCultureIgnoreCase) > -1 || Single.TryParse(word, out s))
						{
							pTag = "CD";
						}
					}
					// rule 3: convert a noun to a past participle if words.get(i) ends with "ed"
					if (pTag.StartsWith("N", StringComparison.CurrentCulture) && word.EndsWith("ed", StringComparison.CurrentCulture))
					{
						pTag = "VBN";
					}
					// rule 4: convert any type to adverb if it ends in "ly";
					if (word.EndsWith("ly", StringComparison.CurrentCulture))
					{
						pTag = "RB";
					}
					// rule 5: convert a common noun (NN or NNS) to a adjective if it ends with "al"
					if (pTag.StartsWith("NN", StringComparison.CurrentCulture) && word.EndsWith("al", StringComparison.CurrentCulture))
					{
						pTag = "JJ";
					}
					// rule 6: convert a noun to a verb if the preceeding work is "would"
					if (i > 0 && pTag.StartsWith("NN", StringComparison.CurrentCulture) && string.Equals(words[i - 1], "would"))
					{
						pTag = "VB";
					}
					// rule 7: if a word has been categorized as a common noun and it ends with "s",
					//         then set its type to plural common noun (NNS)
					if (string.Equals(pTag, "NN") && word.EndsWith("s", StringComparison.CurrentCulture))
					{
						pTag = "NNS";
					}
					// rule 8: convert a common noun to a present participle verb (i.e., a gerand)
					if (pTag.StartsWith("NN", StringComparison.CurrentCulture) && word.EndsWith("ing", StringComparison.CurrentCulture))
					{
						pTag = "VBG";
					}
					// rule 9: Time-based expressions

					result[i] = new WordTag(word, new string[] { pTag });
				}
				else
				{
					// Something bad must have happened
					result[i] = new WordTag(word, new string[] { "UNPROCESSED" });
				}
			});

			return result;
		}

		/// <summary>
		/// Assigns parts of speech to a sentence
		/// </summary>
		/// <param name="sentence"></param>
		/// <returns></returns>
		public IList<WordTag> Tag(string sentence)
		{
			if (string.IsNullOrEmpty(sentence)) return new List<WordTag>();
			var sentenceWords = sentence.Split(' ');
			return Tag(sentenceWords);
		}

		/// <summary>
		/// Assigns parts of speech to a sentence
		/// </summary>
		/// <param name="sentence"></param>
		/// <returns></returns>
		public async void UpdateWord(WordTag corpus)
		{
			await _CorporaRepository.EditCorpusTags(corpus);
		}

		/// <summary>
		/// Clears special chars from start and end of the word
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string RemoveSpecialCharacters(string str)
		{
			if (str.Length == 1) return str;
			var rpl = Regex.Replace(str, "^[^A-Za-z0-9]+|[^A-Za-z0-9]+$", string.Empty);
			return rpl;
		}
    }
}

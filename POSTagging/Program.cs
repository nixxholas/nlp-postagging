using System;
using System.Collections.Generic;
using System.IO;
using POSTagging.Tagging;

namespace POSTagging
{
    class Program
    {
        static void Main(string[] args)
        {
            string sentence = "";
            string result = "";

            Console.WriteLine("Hello World!");

            TagProcessor processor = new TagProcessor();

            processor.LoadJson();

			// Incase you want to see which infinite loop is more efficient..
			// https://stackoverflow.com/questions/20186809/endless-loop-in-c-c
			while (true)
			{
				sentence = Console.ReadLine();

				if (sentence != null && sentence != "")
				{
                    if (sentence.Equals("q")) {
                        break;
                    }

                    IList<WordTag> taggedSentence = processor.Tag(sentence);

                    foreach (WordTag w in taggedSentence) {
                        result += w.Word + " " + string.Join(", ", w.Tag) + " ";
                    }

                    Console.WriteLine("Tagged Sentence: ");
                    Console.WriteLine(result);
                }
            }
        }
    }
}

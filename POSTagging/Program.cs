using System;
using System.IO;

namespace POSTagging
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            TagProcessor processor = new TagProcessor();

            processor.LoadJson();
        }
    }
}

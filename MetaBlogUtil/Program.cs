using System;

namespace GOS.MetaWeblogToPretzelMardown
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var converter = new MetaWeblogToPretzelMardownConverter();
            Console.Write("Working... ");
            converter.ExportAllBlogs();
            Console.WriteLine(" Done");
            Console.ReadKey(true);
        }

    }
}

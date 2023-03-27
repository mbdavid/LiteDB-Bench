using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using LiteDB_Bench;

namespace LiteDB_Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: dotnet run <count>");
                return;
            }

            int count = Convert.ToInt32(args[0]);

            RunTest("LiteDB: default", new LiteDB_Test(count, null));
            RunTest("LiteDB: encrypted", new LiteDB_Test(count, "mypass"));

            RunTest("SQLite: default", new SQLite_Test(count, null));
            RunTest("SQLite: encrypted", new SQLite_Test(count, "mypass"));

            Console.ReadKey();
        }

        static void RunTest(string name, ITest test)
        {
            var title = name + " - " + test.Count + " records";
            Console.WriteLine(title);
            Console.WriteLine("=".PadLeft(title.Length, '='));

            using (test)
            {
                test.Prepare();

                test.Run("Insert", test.Insert);
                test.Run("Bulk", test.Bulk);
                test.Run("Update", test.Update);
                test.Run("Query", test.Query);
                test.Run("Delete", test.Delete);

                Console.WriteLine("FileLength     : " + Math.Round((double)test.FileLength / (double)1024, 2).ToString().PadLeft(5, ' ') + " kb");
            }

            Console.WriteLine();

        }
    }
}

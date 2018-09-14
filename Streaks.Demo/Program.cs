using System;
using System.Collections.Generic;

namespace Streaks.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var warmup = Warmup();

            Console.WriteLine("Press any key to prepare...");
            Console.WriteLine("");
            Console.ReadKey();

            var streak = Streak.Open($@"{Environment.CurrentDirectory}/demo");

            var writer = streak.Writer();
            var reader = streak.Reader();

            var data = new List<byte[]>(1000000);
            var item = new byte[100];

            Console.WriteLine("Press any key to start...");
            Console.WriteLine("");
            Console.ReadKey();

            for (var i = 1; i <= 1000000; i++)
            {
                data.Add(reader.Read(i));

                //writer.Write(item);
                //if (i % 1 == 0) writer.Commit();
            }

            Console.ReadKey();

            Console.WriteLine("");
            Console.WriteLine("Finished");
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();

            Console.WriteLine(warmup);
            Console.WriteLine(data.Count);
            Console.WriteLine(reader.Read(1));
        }

        private static string Warmup()
        {
            var streak = Streak.Open($@"{Environment.CurrentDirectory}/jit");

            using (var w = streak.Writer())
            using (var r = streak.Reader())
            {
                w.Write(new byte[100]);
                w.Write(new byte[100]);
                w.Write(new byte[100]);
                w.Discard();

                w.Write(new byte[100]);
                w.Commit();

                w.Write(new byte[100]);
                w.Write(new byte[100]);
                w.Commit();

                return $"{r.Read(1)}{r.Read(2)}{r.Read(3)}";
            }
        }
    }
}
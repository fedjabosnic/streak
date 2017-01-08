using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Streak.Core;
using Streak.Dsl;

namespace Streak
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            try
            {
                if (args.Contains("-h") || args.Contains("--help"))
                {
                    Usage();
                    return;
                }

                if (args.Length < 2) throw new Exception("Too few parameters specified");

                var count = args.Length;

                var command = args[0];
                var path = args[count - 1];

                var options = args.Skip(1).Take(count - 2).ToList();

                // TODO: Proper parameter parsing with user-friendly errors
                var follow = options.Contains("-f");
                var lines = options.Contains("-n") ? int.Parse(options[options.IndexOf("-n") + 1]) : 10;
                var destination = options.Contains("-d") ? options[options.IndexOf("-d") + 1] : "";

                var streak = new Core.Streak(path);

                switch (command)
                {
                    case "length":
                        {
                            Console.WriteLine(path + " => " + streak.Length);

                            break;
                        }
                    case "head":
                        {
                            var length = streak.Length;
                            var from = 1;
                            var to = length < lines ? length : lines;

                            foreach (var e in streak.Get(from, to))
                            {
                                Console.WriteLine(e.Data);
                            }

                            break;
                        }
                    case "tail":
                        {
                            var length = streak.Length;
                            var from = length < lines ? length : length - lines + 1;
                            var to = follow ? long.MaxValue : from + lines;

                            foreach (var e in streak.Get(from, to, follow))
                            {
                                Console.WriteLine(e.Data);
                            }

                            break;
                        }
                    case "replicate":
                        {
                            var replica = new Core.Streak(destination, writer: true);

                            streak.ReplicateTo(replica);

                            Console.WriteLine($"Replicating '{streak}' to '{destination}'");
                            Console.WriteLine("");

                            while (follow)
                            {
                                Console.WriteLine($"Replicated {replica.Length}/{streak.Length}");
                                Thread.Sleep(1000);
                            }

                            Console.ReadLine();

                            break;
                        }
                    default:
                        throw new Exception($"Unrecognised command '{command}'");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine("");
                Console.WriteLine("Use 'streak -h' for help and usage instructions...");
            }
        }

        internal static void Usage()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("");
            Console.WriteLine("streak [command] [options] [path]");
            Console.WriteLine("");
            Console.WriteLine("[command] - The command to run [length|tail|replicate]");
            Console.WriteLine("[options] - The options for the command:");
            Console.WriteLine("            [-n]: The number of entries to show (tail only)");
            Console.WriteLine("            [-f]: Follow in real time (tail|replicate only)");
            Console.WriteLine("            [-d]: The destination streak (replicate only)");
            Console.WriteLine("[path]    - The fully qualified path to the streak file");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("Show the number of entries in the streak 'c:\\streaks\\aaa'");
            Console.WriteLine("'streak length c:\\streaks\\aaa'");
            Console.WriteLine("");
            Console.WriteLine("Follow events in the streak 'c:\\streaks\\aaa'");
            Console.WriteLine("'streak tail -f c:\\streaks\\aaa'");
            Console.WriteLine("");
            Console.WriteLine("Replicate the entries of streak 'c:\\streaks\\aaa' to streak 'c:\\streaks\\bbb'");
            Console.WriteLine("'streak replicate -d c:\\streaks\\bbb c:\\streaks\\aaa'");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}

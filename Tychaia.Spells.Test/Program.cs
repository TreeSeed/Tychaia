using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Test
{
    class Program
    {
        static void Main(string[] pargs)
        {
            Random r;
            int seed;
            Console.WriteLine("Tychaia Spell Generation Program");
            if (pargs.Length == 1)
                seed = Convert.ToInt32(pargs[0]);
            else
            {
                r = new Random();
                seed = r.Next();
            }
            Console.WriteLine("Random seed is: " + seed);
            r = new Random(seed);

            bool running = true;
            bool locked = false;
            while (running)
            {
                if (locked)
                    r = new Random(seed);
                Console.Write("> ");
                string[] args = Console.ReadLine().Split(' ');
                string cmd = args[0].ToLower();
                try
                {
                    switch (cmd)
                    {
                        case "quit":
                            running = false;
                            break;
                        case "help":
                            Console.WriteLine("Commands are:");
                            Console.WriteLine(" - quit");
                            Console.WriteLine(" - help");
                            Console.WriteLine(" - ss <n>");
                            Console.WriteLine(" - set seed <n>");
                            Console.WriteLine("     Reset the random number generator using seed <n>.");
                            Console.WriteLine(" - sl <b>");
                            Console.WriteLine(" - set lock <b>");
                            Console.WriteLine("     Set whether the seed is locked and reset after each command.");
                            Console.WriteLine(" - gs [<n>]");
                            Console.WriteLine(" - gen spell [<n>]");
                            Console.WriteLine(" - generate spell [<n>]");
                            Console.WriteLine("     Generate <n> (default: 1) spells.");
                            Console.WriteLine(" - gb");
                            Console.WriteLine(" - gen book");
                            Console.WriteLine(" - generate book");
                            Console.WriteLine("     Generate <n> (default: 1) spells.");
                            Console.WriteLine(" - wl");
                            Console.WriteLine(" - weights load");
                            Console.WriteLine("     Load weighting information from file.");
                            Console.WriteLine(" - ws");
                            Console.WriteLine(" - weights save");
                            Console.WriteLine("     Save weighting information from file.");
                            Console.WriteLine(" - wp [<filter>]");
                            Console.WriteLine(" - weights print [<filter>]");
                            Console.WriteLine("     Print weighting information for objects whose name contains <filter>.");
                            Console.WriteLine(" - wc <filter> <value>");
                            Console.WriteLine(" - weights change <filter> <value>");
                            Console.WriteLine("     Set weighting value to <value> for objects whose name contains <filter>.");
                            break;
                        case "ss":
                            HandleSetSeed(Convert.ToInt32(args[1]), out seed, out r);
                            break;
                        case "sl":
                            HandleSetLock(Convert.ToBoolean(args[1]), out locked);
                            break;
                        case "set":
                            switch (args[1].ToLower())
                            {
                                case "seed":
                                    HandleSetSeed(Convert.ToInt32(args[2]), out seed, out r);
                                    break;
                                case "lock":
                                    HandleSetLock(Convert.ToBoolean(args[2]), out locked);
                                    break;
                                default:
                                    Console.WriteLine("Unknown set command.");
                                    break;
                            }
                            break;
                        case "gs":
                            if (args.Length < 2)
                                HandleGenerateSpell(r);
                            else
                                HandleGenerateSpell(r, Convert.ToInt32(args[1]));
                            break;
                        case "gb":
                            HandleGenerateSpellbook(r);
                            break;
                        case "gen":
                        case "generate":
                            switch (args[1].ToLower())
                            {
                                case "spell":
                                    if (args.Length < 3)
                                        HandleGenerateSpell(r);
                                    else
                                        HandleGenerateSpell(r, Convert.ToInt32(args[2]));
                                    break;
                                case "book":
                                    HandleGenerateSpellbook(r);
                                    break;
                                default:
                                    Console.WriteLine("Unknown generate command.");
                                    break;
                            }
                            break;
                        case "wl":
                            WeightManager.LoadWeights();
                            break;
                        case "ws":
                            WeightManager.SaveWeights();
                            break;
                        case "wp":
                            if (args.Length < 2)
                                WeightManager.PrintWeights();
                            else
                                WeightManager.PrintWeights(args[1]);
                            break;
                        case "wc":
                            WeightManager.ChangeWeights(args[1], Convert.ToDouble(args[2]));
                            break;
                        case "weights":
                            switch (args[1].ToLower())
                            {
                                case "load":
                                    WeightManager.LoadWeights();
                                    break;
                                case "save":
                                    WeightManager.SaveWeights();
                                    break;
                                case "print":
                                    if (args.Length < 3)
                                        WeightManager.PrintWeights();
                                    else
                                        WeightManager.PrintWeights(args[2]);
                                    break;
                                case "change":
                                    WeightManager.ChangeWeights(args[2], Convert.ToDouble(args[3]));
                                    break;
                                default:
                                    Console.WriteLine("Unknown weights command.");
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown command.");
                            break;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("IndexOutOfRangeException - Not enough arguments?");
                }
                catch (FormatException)
                {
                    Console.WriteLine("FormatException - Bad argument format (check numerical / boolean)?");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.GetType().FullName);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private static void HandleSetSeed(int seed, out int seedOut, out Random r)
        {
            seedOut = seed;
            r = new Random(seed);
        }

        private static void HandleSetLock(bool locked, out bool lockedOut)
        {
            lockedOut = locked;
        }

        private static void HandleGenerateSpell(Random r, int number = 1)
        {
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine(SpellGenerator.Generate(r.Next()));
            }
        }

        private static void HandleGenerateSpellbook(Random r)
        {
            Console.WriteLine("Spell book: ");
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(" - " + SpellGenerator.Generate(r.Next()));
            }
        }
    }
}

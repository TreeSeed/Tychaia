//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Drawing;
using System;
using System.Collections.Generic;

namespace Tychaia.Puzzle
{
    public class WarfranePuzzle : IPuzzle
    {
        public class Tile
        {
            public int X;
            public int Y;
            public bool[] Line = new bool[] { false, false, false, false, false, false, false };
            public int[] Adj;
            public int ID;
            public Color TileColor = Color.Blue;
            public bool Set = false;
            public WarfranePuzzle Puzzle;
            public int Rotation = 0;

            public Tile(WarfranePuzzle puzzle) : this(0, puzzle)
            {
                X = 200;
                Y = 200;
            }

            public Tile(int id, WarfranePuzzle puzzle)
            {
                X = (int)(Math.Sin(id * Math.PI / 3) * 100 + 200);
                Y = (int)(Math.Cos(id * Math.PI / 3) * 100 + 200);
                ID = id;

                switch (id)
                {
                    case 0:
                        Adj = new int[] { 1, 2, 3, 4, 5, 6 };
                        break;
                    case 1:
                        Adj = new int[] { 0, 2, 6 };
                        break;
                    case 2:
                        Adj = new int[] { 0, 3, 1 };
                        break;
                    case 3:
                        Adj = new int[] { 0, 4, 2 };
                        break;
                    case 4:
                        Adj = new int[] { 0, 5, 3 };
                        break;
                    case 5:
                        Adj = new int[] { 0, 6, 4 };
                        break;
                    case 6:
                        Adj = new int[] { 0, 1, 5 };
                        break;
                }

                Puzzle = puzzle;
            }

            public void Draw(IPuzzleUI ui)
            {
                ui.SetColor(TileColor);
                ui.DrawCircle(X, Y, 50);
                ui.SetColor(Color.Black);

                int c = 0;

                foreach (var a in Adj)
                {
                    if (Line[a] == true)
                    {
                        int b = Rotation + c;
                        ui.DrawLine(X, Y, X + (int)(Math.Sin((b) * Math.PI / 3) * 50), Y + (int)(Math.Cos((b) * Math.PI / 3) * 50));
                    }

                    if (ID != 0)
                    {
                        if (a == 0)
                            c++;
                        else
                            c = -1;
                    }
                    else
                        c++;
                }
            }
        }

        private Color m_Color = Color.Black;
        public List<Tile> TileList = new List<Tile> ();
        private long random = 0;
        private long mod = 0;
        private Random r = new Random();
        private int victories = 0;

        public WarfranePuzzle()
        {
            TileList.Add(new Tile(this));
            for (int a = 1; a < 7; a++)
            {
                TileList.Add(new Tile(a, this));
            }

            Generate();
            /*
             *  4
             * 503
             * 602
             *  1
             *
             */
        }

        public void Generate()
        {
            for (int a = 0; a < 7; a++)
            {
                TileList[a].Set = false;
                TileList[a].Line = new bool[] { false, false, false, false, false, false, false };
            }

            int number = GetRandomRange(random, random, random, random, 3, 7, random + mod);
            mod = r.Next();
            int selection = GetRandomRange(random, random, random, random, 0, 7, random + mod);
            mod = r.Next();

            while (number > 0)
            {
                if (TileList[selection].Set == false)
                {
                    TileList[selection].Set = true;
                    if (selection == 0)
                    {
                        selection = TileList[selection].Adj[GetRandomRange(random, random, random, random, 0, 6, random + mod)];
                    }
                    else
                    {
                        selection = TileList[selection].Adj[GetRandomRange(random, random, random, random, 0, 3, random + mod)];
                    }
                    number--;
                    mod = r.Next();
                }
                else
                {
                    mod = r.Next();
                    selection = GetRandomRange(random, random, random, random, 0, 7, random + mod);
                }
            }

            CheckLinks();
            JumbleRotation();
        }

        public void JumbleRotation()
        {
            foreach (var tile in TileList)
            {
                tile.Rotation = GetRandomRange(random, random, random, random, 0, 6, random + mod);
                mod = r.Next();
            }
        }

        public void CheckLinks()
        {
            for (int a = 0; a < 7; a++)
            {
                if (TileList[a].Set == true)
                    foreach (var b in TileList[a].Adj)
                    {
                        if (TileList[b].Set == true)
                            TileList[a].Line[b] = true;
                    }
            }
        }

        public void DrawUI(IPuzzleUI ui)
        {
            ui.BeginUI();

            ui.DrawText(300, 600, victories.ToString());

            foreach (var Tile in TileList)
                Tile.Draw(ui);

            ui.EndUI();
        }

        public void ClickLeft(int x, int y)
        {
            foreach (var Tile in TileList)
                if (Math.Sqrt(Math.Pow(x - Tile.X, 2) + Math.Pow(y - Tile.Y, 2)) < 50)
            {
                    Tile.Rotation++;
            }

            if (x > 600)
            {
                random = r.Next();
                mod = r.Next();
                Generate();
            }
        }

        public void ClickRight(int x, int y)
        {
            foreach (var Tile in TileList)
                if (Math.Sqrt(Math.Pow(x - Tile.X, 2) + Math.Pow(y - Tile.Y, 2)) < 50)
            {
                Tile.Rotation--;
            }
        }

        public bool Completed
        {
            get
            {
                return false;
            }
        }

        public static int GetRandomRange(long seed, long x, long y, long z, int start, int end, long modifier)
        {
            unchecked
            {
                int a = GetRandomInt(seed, x, y, z, modifier);
                if (a < 0)
                    a += int.MaxValue;

                return a % (end - start) + start;
            }
        }

        public static int GetRandomInt(long seed, long x, long y, long z, long modifier = 0)
        {
            unchecked
            {
                return (int)(GetRandomNumber(seed, x, y, z, modifier) % int.MaxValue);
            }

        }
        private static long GetRandomNumber(long _seed, long x, long y, long z, long modifier)
        {
            /* From: http://stackoverflow.com/questions/2890040/implementing-gethashcode
             * Although we aren't implementing GetHashCode, it's still a good way to generate
             * a unique number given a limited set of fields */
            unchecked
            {
                long seed = (x - 1) * 3661988493967 + (y - 1);
                seed += (x - 2) * 2990430311017;
                seed *= (y - 3) * 14475080218213;
                seed += modifier;
                seed += (y - 4) * 28124722524383;
                seed += (z - 5) * 25905201761893;
                seed *= (x - 6) * 16099760261113;
                seed += (x - 7) * _seed;
                seed *= (y - 8) * _seed;
                seed += (z - 9) * 55497960863;
                seed *= (z - 10) * 611286883423;
                seed += modifier;
                // Prevents the seed from being 0 along an axis.
                seed += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;

                long rng = seed * seed;
                rng += (x - 11) * 2990430311017;
                rng *= (y - 12) * 14475080218213;
                rng *= (z - 13) * 23281823741513;
                rng -= seed * 28124722524383;
                rng *= (x - 14) * 16099760261113;
                rng += seed * _seed;
                rng *= (y - 15) * _seed;
                rng *= (z - 16) * 18193477834921;
                rng += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
                rng += modifier;
                rng += 3661988493967;

                return rng;
            }
        }
    }
}


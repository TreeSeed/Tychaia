// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tychaia.Puzzle
{
    public class WarfranePuzzle : IPuzzle
    {
        private readonly Random m_R = new Random();
        public List<Tile> TileList = new List<Tile>();
        private long m_Mod;
        private long m_Random;

        public WarfranePuzzle()
        {
            this.TileList.Add(new Tile(this));
            for (var a = 1; a < 7; a++)
            {
                this.TileList.Add(new Tile(a, this));
            }

            this.Generate();
            /*
             *  4
             * 503
             * 602
             *  1
             *
             */
        }

        public void DrawUI(IPuzzleUI ui)
        {
            ui.BeginUI();

            ui.DrawText(300, 600, "0");

            foreach (var tile in this.TileList)
                tile.Draw(ui);

            ui.EndUI();
        }

        public void ClickLeft(int x, int y)
        {
            foreach (var tile in this.TileList)
                if (Math.Sqrt(Math.Pow(x - tile.X, 2) + Math.Pow(y - tile.Y, 2)) < 50)
                {
                    tile.Rotation++;
                }

            if (x > 600)
            {
                this.m_Random = this.m_R.Next();
                this.m_Mod = this.m_R.Next();
                this.Generate();
            }
        }

        public void ClickRight(int x, int y)
        {
            foreach (var tile in this.TileList)
                if (Math.Sqrt(Math.Pow(x - tile.X, 2) + Math.Pow(y - tile.Y, 2)) < 50)
                {
                    tile.Rotation--;
                }
        }

        public bool Completed
        {
            get { return false; }
        }

        public void Generate()
        {
            for (var a = 0; a < 7; a++)
            {
                this.TileList[a].Set = false;
                this.TileList[a].Line = new[] { false, false, false, false, false, false, false };
            }

            var number = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 3, 7,
                this.m_Random + this.m_Mod);
            this.m_Mod = this.m_R.Next();
            var selection = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 7,
                this.m_Random + this.m_Mod);
            this.m_Mod = this.m_R.Next();

            while (number > 0)
            {
                if (this.TileList[selection].Set == false)
                {
                    this.TileList[selection].Set = true;
                    if (selection == 0)
                    {
                        selection =
                            this.TileList[selection].Adj[
                                GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 6,
                                    this.m_Random + this.m_Mod)];
                    }
                    else
                    {
                        selection =
                            this.TileList[selection].Adj[
                                GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 3,
                                    this.m_Random + this.m_Mod)];
                    }
                    number--;
                    this.m_Mod = this.m_R.Next();
                }
                else
                {
                    this.m_Mod = this.m_R.Next();
                    selection = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 7,
                        this.m_Random + this.m_Mod);
                }
            }

            this.CheckLinks();
            this.JumbleRotation();
        }

        public void JumbleRotation()
        {
            foreach (var tile in this.TileList)
            {
                tile.Rotation = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 6,
                    this.m_Random + this.m_Mod);
                this.m_Mod = this.m_R.Next();
            }
        }

        public void CheckLinks()
        {
            for (var a = 0; a < 7; a++)
            {
                if (this.TileList[a].Set)
                    foreach (var b in this.TileList[a].Adj)
                    {
                        if (this.TileList[b].Set)
                            this.TileList[a].Line[b] = true;
                    }
            }
        }

        public static int GetRandomRange(long seed, long x, long y, long z, int start, int end, long modifier)
        {
            unchecked
            {
                var a = GetRandomInt(seed, x, y, z, modifier);
                if (a < 0)
                    a += int.MaxValue;

                return a % (end - start) + start;
            }
        }

        public static int GetRandomInt(long seed, long x, long y, long z, long modifier = 0)
        {
            unchecked
            {
                return (int) (GetRandomNumber(seed, x, y, z, modifier) % int.MaxValue);
            }
        }

        private static long GetRandomNumber(long initialSeed, long x, long y, long z, long modifier)
        {
            /* From: http://stackoverflow.com/questions/2890040/implementing-gethashcode
             * Although we aren't implementing GetHashCode, it's still a good way to generate
             * a unique number given a limited set of fields */
            unchecked
            {
                var seed = (x - 1) * 3661988493967 + (y - 1);
                seed += (x - 2) * 2990430311017;
                seed *= (y - 3) * 14475080218213;
                seed += modifier;
                seed += (y - 4) * 28124722524383;
                seed += (z - 5) * 25905201761893;
                seed *= (x - 6) * 16099760261113;
                seed += (x - 7) * initialSeed;
                seed *= (y - 8) * initialSeed;
                seed += (z - 9) * 55497960863;
                seed *= (z - 10) * 611286883423;
                seed += modifier;
                // Prevents the seed from being 0 along an axis.
                seed += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;

                var rng = seed * seed;
                rng += (x - 11) * 2990430311017;
                rng *= (y - 12) * 14475080218213;
                rng *= (z - 13) * 23281823741513;
                rng -= seed * 28124722524383;
                rng *= (x - 14) * 16099760261113;
                rng += seed * initialSeed;
                rng *= (y - 15) * initialSeed;
                rng *= (z - 16) * 18193477834921;
                rng += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
                rng += modifier;
                rng += 3661988493967;

                return rng;
            }
        }

        public class Tile
        {
            public int[] Adj;
            public int ID;
            public bool[] Line = { false, false, false, false, false, false, false };
            public WarfranePuzzle Puzzle;
            public int Rotation = 0;
            public bool Set = false;
            public Color TileColor = Color.Blue;
            public int X;
            public int Y;

            public Tile(WarfranePuzzle puzzle) : this(0, puzzle)
            {
                this.X = 200;
                this.Y = 200;
            }

            public Tile(int id, WarfranePuzzle puzzle)
            {
                this.X = (int) (Math.Sin(id * Math.PI / 3) * 100 + 200);
                this.Y = (int) (Math.Cos(id * Math.PI / 3) * 100 + 200);
                this.ID = id;

                switch (id)
                {
                    case 0:
                        this.Adj = new[] { 1, 2, 3, 4, 5, 6 };
                        break;
                    case 1:
                        this.Adj = new[] { 0, 2, 6 };
                        break;
                    case 2:
                        this.Adj = new[] { 0, 3, 1 };
                        break;
                    case 3:
                        this.Adj = new[] { 0, 4, 2 };
                        break;
                    case 4:
                        this.Adj = new[] { 0, 5, 3 };
                        break;
                    case 5:
                        this.Adj = new[] { 0, 6, 4 };
                        break;
                    case 6:
                        this.Adj = new[] { 0, 1, 5 };
                        break;
                }

                this.Puzzle = puzzle;
            }

            public void Draw(IPuzzleUI ui)
            {
                ui.SetColor(this.TileColor);
                ui.DrawCircle(this.X, this.Y, 50);
                ui.SetColor(Color.Black);

                var c = 0;

                foreach (var a in this.Adj)
                {
                    if (this.Line[a])
                    {
                        var b = this.Rotation + c;
                        ui.DrawLine(this.X, this.Y, this.X + (int) (Math.Sin((b) * Math.PI / 3) * 50),
                            this.Y + (int) (Math.Cos((b) * Math.PI / 3) * 50));
                    }

                    if (this.ID != 0)
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
    }
}

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
    public class DDOPuzzle : IPuzzle
    {
        public class Tile
        {
            public int X;
            public int Y;
            public int Size = 100;
            public bool[] Line = new bool[] { false, false, false, false };
            public int[] Adj;
            public int ID;
            public Color TileColor = Color.Black;
            public bool Set = false;
            public DDOPuzzle Puzzle;
            public int Rotation = 0;

            public Tile(int x, int y, DDOPuzzle puzzle)
            {
                X = x;
                Y = y;
                Puzzle = puzzle;
            }

            public void Draw(IPuzzleUI ui)
            {
                ui.SetColor(TileColor);

                ui.DrawRectangle(X - Size/2, Y - Size/2, X + Size/2, Y + Size/2);

                ui.SetColor(Color.Blue);
                switch(Rotation)
                {
                    case 0:
                        if (Line[0] == true)
                            ui.DrawLine(X, Y, X, Y + Size/2);
                        if (Line[1] == true)
                            ui.DrawLine(X, Y, X + Size/2, Y);
                        if (Line[2] == true)
                            ui.DrawLine(X, Y, X, Y - Size/2);
                        if (Line[3] == true)
                            ui.DrawLine(X, Y, X - Size/2, Y);
                        break;
                    case 1:
                        if (Line[1] == true)
                            ui.DrawLine(X, Y, X, Y + Size/2);
                        if (Line[2] == true)
                            ui.DrawLine(X, Y, X + Size/2, Y);
                        if (Line[3] == true)
                            ui.DrawLine(X, Y, X, Y - Size/2);
                        if (Line[0] == true)
                            ui.DrawLine(X, Y, X - Size/2, Y);
                        break;
                    case 2:
                        if (Line[2] == true)
                            ui.DrawLine(X, Y, X, Y + Size/2);
                        if (Line[3] == true)
                            ui.DrawLine(X, Y, X + Size/2, Y);
                        if (Line[0] == true)
                            ui.DrawLine(X, Y, X, Y - Size/2);
                        if (Line[1] == true)
                            ui.DrawLine(X, Y, X - Size/2, Y);
                        break;
                    case 3:
                        if (Line[3] == true)
                            ui.DrawLine(X, Y, X, Y + Size/2);
                        if (Line[0] == true)
                            ui.DrawLine(X, Y, X + Size/2, Y);
                        if (Line[1] == true)
                            ui.DrawLine(X, Y, X, Y - Size/2);
                        if (Line[2] == true)
                            ui.DrawLine(X, Y, X - Size/2, Y);
                        break;
                }
            }
        }

        private Color m_Color = Color.Black;
        public List<Tile> TileList = new List<Tile> ();
        private long random = 0;
        private long mod = 0;
        private Random r = new Random();
        private int victories = 0;

        public DDOPuzzle()
        {
            for (int i = -2; i < 3; i++)
                for (int j = -2; j < 3; j++)
                    TileList.Add(new Tile(300 + i * 100, 300 + j * 100, this));


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
            foreach(Tile tile in TileList)
            {
                int amount = GetRandomRange(random, random, random, random, 1, 4, random + mod);
                mod = r.Next();

                for (int i = 0; i < amount; i++)
                {
                    int selected = GetRandomRange(random, random, random, random, 0, 4, random + mod);

                    while (tile.Line[selected] == true)
                    {
                        mod = r.Next();
                        selected = GetRandomRange(random, random, random, random, 0, 4, random + mod);
                    }

                    tile.Line[selected] = true;
                    mod = r.Next();
                }
            }
            MakePath();
            JumbleRotation();
        }

        public void MakePath()
        {
            int connect = 0;
            foreach(Tile tile in TileList)
            {
                tile.Line[connect] = true;

                int selected = GetRandomRange(random, random, random, random, 0, 2, random + mod);
                mod = r.Next();
                for (int i = 2; i < 4; i++)
                {
                    int j = i + selected;
                    if (j > 3)
                        j -= 2;

                    if (tile.Line[j] == true)
                        connect = j;
                }
            }
        }

        public void JumbleRotation()
        {
            foreach (var tile in TileList)
            {
                tile.Rotation = GetRandomRange(random, random, random, random, 0, 4, random + mod);
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

            //ui.DrawText(300, 600, victories.ToString());

            foreach (var Tile in TileList)
                Tile.Draw(ui);


            ui.EndUI();
        }

        public void ClickLeft(int x, int y)
        {
            foreach (var Tile in TileList)
                if (x - Tile.X < 50 && Tile.X - x < 50 && y - Tile.Y < 50 && Tile.Y - y < 50)
            {
                if (Tile.Rotation == 3)
                    Tile.Rotation = 0;
                else
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
                if (x - Tile.X < 50 && Tile.X - x < 50 && y - Tile.Y < 50 && Tile.Y - y < 50)
            {
                if (Tile.Rotation == 0)
                    Tile.Rotation = 3;
                else
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


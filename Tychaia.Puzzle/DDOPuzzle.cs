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
    public class DDOPuzzle : IPuzzle
    {
        private readonly Random m_R = new Random();
        public List<Tile> TileList = new List<Tile>();
        private long m_Mod;
        private long m_Random;

        public DDOPuzzle()
        {
            for (var i = -2; i < 3; i++)
                for (var j = -2; j < 3; j++)
                    this.TileList.Add(new Tile(300 + i * 100, 300 + j * 100, this));


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

            //ui.DrawText(300, 600, victories.ToString());

            foreach (var tile in this.TileList)
                tile.Draw(ui);


            ui.EndUI();
        }

        public void ClickLeft(int x, int y)
        {
            foreach (var tile in this.TileList)
                if (x - tile.X < 50 && tile.X - x < 50 && y - tile.Y < 50 && tile.Y - y < 50)
                {
                    if (tile.Rotation == 3)
                        tile.Rotation = 0;
                    else
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
                if (x - tile.X < 50 && tile.X - x < 50 && y - tile.Y < 50 && tile.Y - y < 50)
                {
                    if (tile.Rotation == 0)
                        tile.Rotation = 3;
                    else
                        tile.Rotation--;
                }
        }

        public bool Completed
        {
            get { return false; }
        }

        public void Generate()
        {
            foreach (var tile in this.TileList)
            {
                var amount = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 1, 4,
                    this.m_Random + this.m_Mod);
                this.m_Mod = this.m_R.Next();

                for (var i = 0; i < amount; i++)
                {
                    var selected = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 4,
                        this.m_Random + this.m_Mod);

                    while (tile.Line[selected])
                    {
                        this.m_Mod = this.m_R.Next();
                        selected = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 4,
                            this.m_Random + this.m_Mod);
                    }

                    tile.Line[selected] = true;
                    this.m_Mod = this.m_R.Next();
                }
            }
            this.MakePath();
            this.JumbleRotation();
        }

        public void MakePath()
        {
            var connect = 0;
            foreach (var tile in this.TileList)
            {
                tile.Line[connect] = true;

                var selected = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 2,
                    this.m_Random + this.m_Mod);
                this.m_Mod = this.m_R.Next();
                for (var i = 2; i < 4; i++)
                {
                    var j = i + selected;
                    if (j > 3)
                        j -= 2;

                    if (tile.Line[j])
                        connect = j;
                }
            }
        }

        public void JumbleRotation()
        {
            foreach (var tile in this.TileList)
            {
                tile.Rotation = GetRandomRange(this.m_Random, this.m_Random, this.m_Random, this.m_Random, 0, 4,
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
            public bool[] Line = { false, false, false, false };
            public DDOPuzzle Puzzle;
            public int Rotation = 0;
            public bool Set = false;
            public int Size = 100;
            public Color TileColor = Color.Black;
            public int X;
            public int Y;

            public Tile(int x, int y, DDOPuzzle puzzle)
            {
                this.X = x;
                this.Y = y;
                this.Puzzle = puzzle;
            }

            public void Draw(IPuzzleUI ui)
            {
                ui.SetColor(this.TileColor);

                ui.DrawRectangle(this.X - this.Size / 2, this.Y - this.Size / 2, this.X + this.Size / 2,
                    this.Y + this.Size / 2);

                ui.SetColor(Color.Blue);
                switch (this.Rotation)
                {
                    case 0:
                        if (this.Line[0])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y + this.Size / 2);
                        if (this.Line[1])
                            ui.DrawLine(this.X, this.Y, this.X + this.Size / 2, this.Y);
                        if (this.Line[2])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y - this.Size / 2);
                        if (this.Line[3])
                            ui.DrawLine(this.X, this.Y, this.X - this.Size / 2, this.Y);
                        break;
                    case 1:
                        if (this.Line[1])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y + this.Size / 2);
                        if (this.Line[2])
                            ui.DrawLine(this.X, this.Y, this.X + this.Size / 2, this.Y);
                        if (this.Line[3])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y - this.Size / 2);
                        if (this.Line[0])
                            ui.DrawLine(this.X, this.Y, this.X - this.Size / 2, this.Y);
                        break;
                    case 2:
                        if (this.Line[2])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y + this.Size / 2);
                        if (this.Line[3])
                            ui.DrawLine(this.X, this.Y, this.X + this.Size / 2, this.Y);
                        if (this.Line[0])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y - this.Size / 2);
                        if (this.Line[1])
                            ui.DrawLine(this.X, this.Y, this.X - this.Size / 2, this.Y);
                        break;
                    case 3:
                        if (this.Line[3])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y + this.Size / 2);
                        if (this.Line[0])
                            ui.DrawLine(this.X, this.Y, this.X + this.Size / 2, this.Y);
                        if (this.Line[1])
                            ui.DrawLine(this.X, this.Y, this.X, this.Y - this.Size / 2);
                        if (this.Line[2])
                            ui.DrawLine(this.X, this.Y, this.X - this.Size / 2, this.Y);
                        break;
                }
            }
        }
    }
}

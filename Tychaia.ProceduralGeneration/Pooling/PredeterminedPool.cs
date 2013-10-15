// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    public class PredeterminedPool : IPool
    {
        private int[][] m_52_7744Int;
        private int[] m_278784Int;
        private Cell[] m_278784Cell;
        private int m_52_Counter;
        private bool m_278784Int_Used;
        private bool m_278784Cell_Used;

        public PredeterminedPool()
        {
            this.m_52_7744Int = new int[52][];
            for (var i = 0; i < this.m_52_7744Int.Length; i++)
                this.m_52_7744Int[i] = new int[7744];
            this.m_278784Int = new int[278784];
            this.m_278784Cell = new Cell[278784];
            this.Begin();
        }

        public void Begin()
        {
            this.m_52_Counter = 0;
            this.m_278784Int_Used = false;
            this.m_278784Cell_Used = false;
        }

        public dynamic Get(Type type, int size)
        {
            if (type == typeof(int))
            {
                if (size == 7744)
                {
                    if (this.m_52_Counter < 52)
                        return this.m_52_7744Int[this.m_52_Counter++];
                    Console.WriteLine("WARNING: Predetermined pool did not have a 52nd int array.");
                }
                else if (size == 278784)
                {
                    if (!this.m_278784Int_Used)
                    {
                        this.m_278784Int_Used = true;
                        return this.m_278784Int;
                    }
                    
                    Console.WriteLine("WARNING: Predetermined pool did not have another 278784 int array.");
                }
                else
                {
                    Console.WriteLine("WARNING: Predetermined pool was asked for non-standard int array size.");
                }
            }
            else if (type == typeof(Cell))
            {
                if (size == 278784)
                {
                    if (!this.m_278784Cell_Used)
                    {
                        this.m_278784Cell_Used = true;
                        return this.m_278784Cell;
                    }
                    
                    Console.WriteLine("WARNING: Predetermined pool did not have another 278784 cell array.");
                }
                else
                {
                    Console.WriteLine("WARNING: Predetermined pool was asked for non-standard Cell array size.");
                }
            }
            
            return Activator.CreateInstance(type, size);
        }

        public void Release(dynamic value)
        {
        }

        public void End()
        {
        }
    }
}

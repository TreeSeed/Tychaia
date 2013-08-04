using System;

namespace Tychaia
{
    public interface ICellRenderOrderCalculator
    {
        int[] CalculateCellRenderOrder(int renderWidth, int renderHeight);
    }
}


using System;

namespace Tychaia
{
    public class DefaultCellRenderOrderCalculator : ICellRenderOrderCalculator
    {
        public int[] CalculateCellRenderOrder(int renderWidth, int renderHeight)
        {
            /*               North
             *        0  1  2  3  4  5  6
             *        1  2  3  4  5  6  7
             *        2  3  4  5  6  7  8
             *  East  3  4  5  6  7  8  9  West
             *        4  5  6  7  8  9  10
             *        5  6  7  8  9  10 11
             *        6  7  8  9  10 11 12
             *               South
             *
             * Start value is always 0.
             * Last value is (MaxX + MaxY).
             * This is the AtkValue.
             *
             * We attack from the left side of the render first
             * with (X: 0, Y: AtkValue) until Y would be less than
             * half of AtkValue.
             *
             * We then attack from the right side of the render
             * with (X: AtkValue, Y: 0) until X would be less than
             * half of AtkValue - 1.
             *
             * If we are attacking from the left, but Y is now
             * greater than MaxY, then we are over half-way and are
             * now starting at the bottom of the grid.
             *
             * In this case, we start with (X: AtkValue - MaxY, Y: MaxY)
             * and continue until we reach the same conditions that
             * apply normally.  The same method applies to the right hand
             * side where we start with (X: MaxX, Y: AtkValue - MaxX).
             *
             */
             
            int[] result = new int[renderWidth * renderHeight];
            int count = 0;
            int start = 0;
            int maxx = renderWidth - 1;
            int maxy = renderHeight - 1;
            int last = maxx + maxy;
            int x, y;

            for (int atk = start; atk <= last; atk++)
            {
                // Attack from the left.
                if (atk < maxy)
                {
                    x = 0;
                    y = atk;
                }
                else
                {
                    x = atk - maxy;
                    y = maxy;
                }
                while (y > atk / 2)
                    result[count++] = y-- * renderWidth + x++;

                // Attack from the right.
                if (atk < maxx)
                {
                    x = atk;
                    y = 0;
                }
                else
                {
                    x = maxx;
                    y = atk - maxx;
                }
                while (y <= atk / 2)
                    result[count++] = y++ * renderWidth + x--;
            }

            return result;
        }
    }
}


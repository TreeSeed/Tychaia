// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
// @generated
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class FrameProfileInfo
    {
        public int Entities;
        public int FPS;
        public int FunctionCalls;
        public double LastFrameLength;
        public long VirtualMemory;
        public int SendNetworkOps;
        public int ReceiveNetworkOps;
    }
    
    public class TychaiaProfilerEntityUtil
    {
        public void RenderMaximums(I2DRenderUtilities _2DRenderUtilities, IRenderContext renderContext, FontAsset font, List<FrameProfileInfo> info)
        {
            Action<int, string, Color> drawMaximum = (offset, maximum, color) => 
                _2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(4 + offset, 4),
                    maximum,
                    font,
                    textColor: color);
            
            var lastEntities = info.Select(x => x.Entities).Last();
            var lastFPS = info.Select(x => x.FPS).Last();
            var lastFunctionCalls = info.Select(x => x.FunctionCalls).Last();
            var lastLastFrameLength = info.Select(x => x.LastFrameLength).Last();
            var lastVirtualMemory = info.Select(x => x.VirtualMemory).Last();
            var lastSendNetworkOps = info.Select(x => x.SendNetworkOps).Last();
            var lastReceiveNetworkOps = info.Select(x => x.ReceiveNetworkOps).Last();
            drawMaximum(0, lastEntities.ToString(CultureInfo.InvariantCulture) + "", Color.Orange);
            drawMaximum(42, lastFPS.ToString(CultureInfo.InvariantCulture) + "FPS", Color.Yellow);
            drawMaximum(84, lastFunctionCalls.ToString(CultureInfo.InvariantCulture) + "", Color.Green);
            drawMaximum(126, lastLastFrameLength.ToString(CultureInfo.InvariantCulture) + "ms", Color.White);
            drawMaximum(168, lastVirtualMemory.ToString(CultureInfo.InvariantCulture) + "MB", Color.Magenta);
            drawMaximum(210, lastSendNetworkOps.ToString(CultureInfo.InvariantCulture) + "", Color.DarkSalmon);
            drawMaximum(252, lastReceiveNetworkOps.ToString(CultureInfo.InvariantCulture) + "", Color.Cyan);
        }
    
        public void RenderLines(IGameContext gameContext, List<FrameProfileInfo> info)
        {
            Action<int, Func<FrameProfileInfo, double>, double, VertexPositionColor[], Color, int> addToLine =
                (i, value, maximum, vertexList, color, offset) =>
                {
                    var sample = info[i];
                    vertexList[i] = new VertexPositionColor(
                        new Vector3(i, (float) (224 - (value(sample) / (maximum < 1 ? 1 : maximum)) * (200 - offset)), 0),
                        color);
                };
            Action<VertexPositionColor[], short[]> renderLine = (vertexList, lsi) =>
            {
                if (vertexList.Length == 0)
                    return;
                gameContext.Graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineStrip,
                    vertexList,
                    0,
                    vertexList.Length,
                    lsi,
                    0,
                    lsi.Length - 1);
            };
            
            var maximumEntities = info.Select(x => x.Entities).Max();
            var lineEntities = new VertexPositionColor[info.Count - 1];
            var maximumFPS = info.Select(x => x.FPS).Max();
            var lineFPS = new VertexPositionColor[info.Count - 1];
            var maximumFunctionCalls = info.Select(x => x.FunctionCalls).Max();
            var lineFunctionCalls = new VertexPositionColor[info.Count - 1];
            var maximumLastFrameLength = info.Select(x => x.LastFrameLength).Max();
            var lineLastFrameLength = new VertexPositionColor[info.Count - 1];
            var maximumVirtualMemory = info.Select(x => x.VirtualMemory).Max();
            var lineVirtualMemory = new VertexPositionColor[info.Count - 1];
            var maximumSendNetworkOps = info.Select(x => x.SendNetworkOps).Max();
            var lineSendNetworkOps = new VertexPositionColor[info.Count - 1];
            var maximumReceiveNetworkOps = info.Select(x => x.ReceiveNetworkOps).Max();
            var lineReceiveNetworkOps = new VertexPositionColor[info.Count - 1];
            var sixteenMillisecondsLine = new VertexPositionColor[info.Count - 1];
            var lineStripIndices = new short[info.Count - 1];
            for (short i = 0; i < info.Count - 1; i++)
            {
                lineStripIndices[i] = i;
                addToLine(i, x => x.Entities, maximumEntities, lineEntities, Color.Orange, 0);
                addToLine(i, x => x.FPS, maximumFPS, lineFPS, Color.Yellow, 4);
                addToLine(i, x => x.FunctionCalls, maximumFunctionCalls, lineFunctionCalls, Color.Green, 8);
                addToLine(i, x => x.LastFrameLength, maximumLastFrameLength, lineLastFrameLength, Color.White, 12);
                addToLine(i, x => x.VirtualMemory, maximumVirtualMemory, lineVirtualMemory, Color.Magenta, 16);
                addToLine(i, x => x.SendNetworkOps, maximumSendNetworkOps, lineSendNetworkOps, Color.DarkSalmon, 20);
                addToLine(i, x => x.ReceiveNetworkOps, maximumReceiveNetworkOps, lineReceiveNetworkOps, Color.Cyan, 24);
                addToLine(i, x => 1000.0 / 60.0, maximumLastFrameLength, sixteenMillisecondsLine, Color.Red, 20);
            }
            renderLine(lineEntities, lineStripIndices);
            renderLine(lineFPS, lineStripIndices);
            renderLine(lineFunctionCalls, lineStripIndices);
            renderLine(lineLastFrameLength, lineStripIndices);
            renderLine(lineVirtualMemory, lineStripIndices);
            renderLine(lineSendNetworkOps, lineStripIndices);
            renderLine(lineReceiveNetworkOps, lineStripIndices);
            renderLine(sixteenMillisecondsLine, lineStripIndices);
        }
    }
}

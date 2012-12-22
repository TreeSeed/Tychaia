using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL;
using Tychaia.ProceduralGeneration;
using TychaiaWorldGenWebsite;
using TychaiaWorldGenViewer.Flow;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

public class Program
{
    public static void Main(int x, int y, int width, int height, Layer layer)
    {
        dynamic document = Builtins.Global["document"];
        dynamic window = Builtins.Global["window"];

        var canvas = document.getElementById("canvas");
        var ctx = canvas.getContext("2d");

        //var x = Convert.ToInt32(args[0]);
        //var y = Convert.ToInt32(args[1]);
        //var width = Convert.ToInt32(args[2]);
        //var height = Convert.ToInt32(args[3]);

        // Use test layer for now, but later deserialize.
        var imageData = ctx.createImageData(width, height);
        LayerGenerationHTML5.X = x;
        LayerGenerationHTML5.Y = y;
        LayerGenerationHTML5.DrawLayerToImageData(imageData, layer, width, height);
        ctx.putImageData(imageData, x, y);
    }
}

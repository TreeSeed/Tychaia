using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIL;
using Tychaia.ProceduralGeneration;
using TychaiaWorldGenWebsite;

public class Program
{
    public static void Main(string[] args)
    {
        dynamic document = Builtins.Global["document"];
        dynamic window = Builtins.Global["window"];

        var canvas = document.getElementById("canvas");
        var ctx = canvas.getContext("2d");

        var x = Convert.ToInt32(args[0]);
        var y = Convert.ToInt32(args[1]);
        var width = Convert.ToInt32(args[2]);
        var height = Convert.ToInt32(args[3]);

        // Use test layer for now, but later deserialize.
        LayerInitialLand land = new LayerInitialLand(1000);
        var imageData = ctx.createImageData(width, height);
        LayerGenerationHTML5.X = x;
        LayerGenerationHTML5.Y = y;
        LayerGenerationHTML5.DrawLayerToImageData(imageData, land, width, height);
        ctx.putImageData(imageData, x, y);
    }
}

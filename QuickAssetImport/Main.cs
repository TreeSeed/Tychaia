using System.IO;
using System.Text;
using Protogame;
using System;

namespace QuickAssetImport
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("input.xnb output.asset Arial 12");
                return;
            }
            
            var fontSaver = new FontAssetSaver();
            var rawSaver = new RawAssetSaver();
            using (BinaryReader b = new BinaryReader(File.Open(args[0], FileMode.Open)))
            {
                var font = new FontAsset(
                    null,
                    null,
                    args[1],
                    args[2],
                    Convert.ToInt32(args[3]),
                    b.ReadBytes(int.MaxValue));
                rawSaver.SaveRawAsset(args[1], fontSaver.Handle(font));
            }
        }
    }
}
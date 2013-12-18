// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using ManyConsole;
using Protogame;

namespace TychaiaTool
{
    public class QuickAssetImportCommand : ConsoleCommand
    {
        private string m_InputFile;
        private string m_OutputFile;
        private string m_AssetType;
        private string m_FontName;
        private int? m_FontSize;

        public QuickAssetImportCommand()
        {
            this.IsCommand("quick-asset-import", "Import XNB files to Protogame assets");
            this.HasRequiredOption("i|input=", "The input XNB file", x => this.m_InputFile = x);
            this.HasRequiredOption("o|output=", "The output asset file", x => this.m_OutputFile = x);
            this.HasRequiredOption("t|type=", "The asset type (one of: 'font')", x => this.m_AssetType = x);
            this.HasOption("font-name:", "The font name for font assets", x => this.m_FontName = x);
            this.HasOption("font-size:", "The font size for font assets", (int x) => this.m_FontSize = x);
        }

        public override int Run(string[] remainingArguments)
        {
            if (this.m_AssetType == "font")
            {
                if (this.m_FontName == null || this.m_FontSize == null)
                {
                    Console.WriteLine("Missing font name or font size.");
                    return 1;
                }
            }

            Console.WriteLine("Not implemented.");
            return 1;
        }
    }
}


// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Tychaia
{
    using Protogame;

    public class ReportCommand : ICommand
    {
        private readonly IAssetManager m_AssetManager;

        public ReportCommand(IAssetManagerProvider assetManagerProvider)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
        }

        public string[] Names
        {
            get
            {
                return new[] { "report" };
            }
        }

        public string[] Descriptions
        {
            get
            {
                return new[] { "Report an issue, including a screenshot." };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            var worldManager = (TychaiaWorldManager)gameContext.WorldManager;

            worldManager.CaptureNextFrame(
                gameContext,
                CrashReport.CrashReporter.RecordScreenshot);

            return "Next frame will be captured";
        }
    }
}
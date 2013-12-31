// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using CrashReport;
using Protogame;

namespace Tychaia
{
    public class ReportCommand : ICommand
    {
        private readonly ICaptureService m_CaptureService;

        public ReportCommand(ICaptureService captureService)
        {
            this.m_CaptureService = captureService;
        }

        public string[] Descriptions
        {
            get
            {
                return new[] { "Report an issue, including a screenshot." };
            }
        }

        public string[] Names
        {
            get
            {
                return new[] { "report" };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            this.m_CaptureService.CaptureFrame(gameContext, CrashReporter.RecordScreenshot);

            return "Next frame will be captured";
        }
    }
}
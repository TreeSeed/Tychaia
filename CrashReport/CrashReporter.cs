// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Phabricator.Conduit;

namespace CrashReport
{
    public static class CrashReporter
    {
        // Accepts an exception then creates a task in Phabricator
        public static void Record(Exception e)
        {
            // Fetch all system info from the Exception
            CollectSystemInfo.GetSystemInfo();

            // Gather info from the .NET APIs (research)

            // Check user information from registry
            // For now always use default
            var username = ConfigurationManager.AppSettings["generic-username"];
            var certificate = ConfigurationManager.AppSettings["generic-certificate"];
            var clientPHID = ConfigurationManager.AppSettings["generic-PHID"];

            // Get stack trace information.
            var st = new StackTrace(e, true);
            var frames = st.GetFrames();

            // Construct the message.
            var formattedStackTrace = @"| Filename | Line Number | Column Number
| -----  | -----  | -----
";
            foreach (var frame in frames)
            {
                formattedStackTrace += "| " + frame.GetFileName() + " | " + frame.GetFileLineNumber() + " | " + frame.GetFileColumnNumber() + "\r\n";
            }

            var message = @"**Exception:**
" + e.GetType().FullName + ": " + e.Message + @"

**Stack Trace:**
" + formattedStackTrace + @"

**Source:**
" + e.Source + @"

**Method:**
" + e.TargetSite.DeclaringType.FullName + "." + e.TargetSite.Name + @"

**Full Information:**
```
" + e.ToString() + @"
```
";

            // Iterate over the frames extracting the information you need
            var firstFrame = frames.FirstOrDefault();
            var source = e.Source + " (no location information)";
            if (firstFrame != null)
            {
                source = firstFrame.GetFileName() + ":" + firstFrame.GetFileLineNumber();
            }

            // Create Phabricator task
            var client = new ConduitClient("http://code.redpointsoftware.com.au/api/");
            client.User = username;
            client.Certificate = certificate;
            var uri = client.Do("maniphest.createtask", new
            {
                title = e.GetType().FullName + " in " + source,
                description = message,
                ccPHIDs = new string[] { "PHID-USER-4bruxoj4jpjrmz6invrc", "PHID-USER-7ebhodqpyep5kh7q56wn" /* add user phid here */ },
                priority = 90,
                projectPHIDs = new string[] { "PHID-PROJ-3ahdqqipg3rgo7bk4oqo", "PHID-PROJ-4msjmfn2aosxjjygpoa4" }
            }).uri;

            // Notify user of task url
            new CrashReportForm(uri).ShowDialog();

            // Restart Tychaia
        }

        // A method that captures information about the client's computer
        private static void CaptureInformation()
        {
        }
    }
}

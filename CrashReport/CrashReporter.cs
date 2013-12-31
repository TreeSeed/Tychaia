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
        private const string USERNAME = "crash-reporter";
        private const string CERTIFICATE = "y3jplv35imfesns5wxin7i6iv6jlqnasfs6mjwvvgp6mfnkagkrp6v2roif53gtcp56tuf3vd3dlb2vaue5radybntoidyrfk7teifi6u4lxotlev7go45ebz7brmlkgeruta53e2yi54vacsgbiqd7lty2pn3hsyewnwkwmanxyqjdkwvujg2pelojwjr567hco3nn4o64pfmlddhvcn4utnpu6zdeuig5fua6g6calljcdlev53v5aaptv6gg";
        private const string CLIENTPHID = "PHID-USER-qeop3bethxve7d3dcdqc";

        private static readonly string[] CCPHIDS = new string[] { "PHID-USER-4bruxoj4jpjrmz6invrc", "PHID-USER-7ebhodqpyep5kh7q56wn" };
        private static readonly string[] PROJECTPHIDS = new string[] { "PHID-PROJ-3ahdqqipg3rgo7bk4oqo", "PHID-PROJ-4msjmfn2aosxjjygpoa4" };

        // Accepts an exception then creates a task in Phabricator
        public static void Record(Exception e, bool noPrompt = false)
        {
            // Fetch all system info from the Exception
            SystemInfo s = CollectSystemInfo.GetSystemInfo();
            
            // Check user information from registry
            // For now always use default
            var username = USERNAME;
            var certificate = CERTIFICATE;
            var clientPHID = CLIENTPHID;

            // Get stack trace information.
            var st = new StackTrace(e, true);
            var frames = st.GetFrames();
            var formattedStackTrace = string.Empty;

            try
            {
                // Construct the message.
                formattedStackTrace += @"| Type | Method | Filename | Line Number | Column Number
| ----- | ----- | ----- | -----  | -----
";

                // Develop the crash report
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    var type = method == null ? null : method.DeclaringType;
                    var name = type == null ? null : type.FullName;
                    var file = frame.GetFileName();
                    var line = frame.GetFileLineNumber();
                    var column = frame.GetFileColumnNumber();

                    formattedStackTrace += "| " + 
                        name + " | " + 
                        method + " | " +
                        file + " | " +
                        line + " | " +
                        column + "\r\n";
                }
            }
            catch
            {
                formattedStackTrace = "Error collecting stack trace.";
            }

            // Create a conduit client
            var client = new ConduitClient("https://code.redpointsoftware.com.au/api/");
            client.User = username;
            client.Certificate = certificate;

            // Check if a task already exists.
            dynamic searchResult = client.Do(
                "maniphest.query",
                new
                {
                    fullText = "\"" + e.ToString().Replace("\r\n", string.Empty) + "\"",
                    status = "status-open",
                    projectPHIDs = new string[] { "PHID-PROJ-3ahdqqipg3rgo7bk4oqo", "PHID-PROJ-4msjmfn2aosxjjygpoa4" }
                });

            var uri = string.Empty;
            KeyValuePair<string, object>? task = null;

            foreach (var result in searchResult)
            {
                task = result;
                break;
            }

            if (task != null)
            {
                dynamic realTask = task.Value.Value;
                var id = realTask.id;
                var ccPHIDs = realTask.ccPHIDs;
                uri = realTask.uri;
                //// Add this client's ccPHID

                // Update that task with users system information
                client.Do(
                    "maniphest.update",
                    new
                    {
                        id = id,
                        comments = s.ToString()
                    });
            }
            else
            {
                var methodPrefix = string.Empty;
                var methodName = string.Empty;
                if (e.TargetSite != null)
                {
                    methodName = e.TargetSite.Name;
                    if (e.TargetSite.DeclaringType != null)
                    {
                        methodPrefix = e.TargetSite.DeclaringType.FullName;
                    }
                }

                var message = @"**Exception:**
" + e.GetType().FullName + ": " + e.Message + @"

**Stack Trace:**
" + formattedStackTrace + @"

**Source:**
" + e.Source + @"

**Method:**
" + methodPrefix + "." + methodName + @"

**Full Information:**
```lang=none, lines=20
" + e.ToString() + @"
```

**System Information:**
" + s.ToString();

                // Iterate over the frames extracting the information you need
                var firstFrame = frames.FirstOrDefault();
                var source = e.Source + " (no location information)";
                if (firstFrame != null)
                {
                    source = (string.IsNullOrEmpty(firstFrame.GetFileName()) ? firstFrame.GetMethod().ToString() : firstFrame.GetFileName()) + ":" + firstFrame.GetFileLineNumber();
                }

                // Create Phabricator task
                uri = client.Do(
                    "maniphest.createtask",
                    new
                    {
                        title = e.GetType().FullName + " in " + source,
                        description = message,
                        ccPHIDs = CCPHIDS,
                        priority = 100,
                        projectPHIDs = PROJECTPHIDS
                    }).uri;
            }

            if (!noPrompt)
            {
                // Notify user of task url
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                }
                catch
                {
                }

                new CrashReportForm(uri).ShowDialog();

                // Restart Tychaia
            }
        }

        /// <summary>
        /// Accepts a PNG as a stream of bytes and reports an issue to Phabricator with the screenshot
        /// attached.  This is used so that players can report shader and rendering issues.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        public static void RecordScreenshot(byte[] bytes)
        {
            var base64 = Convert.ToBase64String(bytes);

            // Fetch all system info from the Exception
            var s = CollectSystemInfo.GetSystemInfo();

            // Create a conduit client
            var client = new ConduitClient("https://code.redpointsoftware.com.au/api/");
            client.User = USERNAME;
            client.Certificate = CERTIFICATE;

            // Upload the screenshot.
            var result = client.Do(
                "file.upload",
                new
                {
                    data_base64 = base64,
                    name = "screenshot.png"
                });
            var fileInfo = client.Do(
                "file.info",
                new
                {
                    phid = result.Value
                });

            // Create the message.
            var message = @"**Attached Screenshot:**
{" + fileInfo["objectName"].Value + @"}

**System Information:**
" + s.ToString();

            // Create the task.
            var uri = client.Do(
                "maniphest.createtask",
                new
                {
                    title = "Rendering issue report (screenshot attached)",
                    description = message,
                    ccPHIDs = CCPHIDS,
                    priority = 100,
                    projectPHIDs = PROJECTPHIDS
                }).uri;

            // Notify user of task url
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch
            {
            }

            new CrashReportForm(uri.Value).ShowDialog();
        }
    }
}

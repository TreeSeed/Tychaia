using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;

namespace Tychaia.Website.Controllers
{
    public class DownloadController : Controller
    {
        public ActionResult Index()
        {
            return View(new {
                BuildServerOnline = IsBuildServerOnline()
            });
        }

        private class MyWebClient : WebClient
        {
            public MyWebClient()
            {
            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 1000;
                return w;
            }
        }

        private bool IsBuildServerOnline()
        {
            if (!Directory.Exists("App_Cache"))
                Directory.CreateDirectory("App_Cache");
            if (System.IO.File.Exists("App_Cache/build_server_last_check.txt") &&
                System.IO.File.Exists("App_Cache/build_server_last_status.txt"))
            {
                string lastStatus = "";
                DateTime lastCheckDate;
                using (var reader = new StreamReader("App_Cache/build_server_last_check.txt"))
                {
                    if (!DateTime.TryParse(reader.ReadToEnd(), out lastCheckDate))
                        lastCheckDate = new DateTime(2000, 1, 1);
                }
                using (var reader = new StreamReader("App_Cache/build_server_last_status.txt"))
                {
                    lastStatus = reader.ReadToEnd();
                }
                if ((DateTime.Now - lastCheckDate).TotalMinutes < 15)
                    return lastStatus == "online";
            }

            var online = true;
            var client = new MyWebClient();
            try
            {
                client.DownloadString("http://build.redpointsoftware.com.au/");
            }
            catch (WebException)
            {
                online = false;
            }

            using (var writer = new StreamWriter("App_Cache/build_server_last_check.txt"))
            {
                writer.Write(DateTime.Now);
            }
            using (var writer = new StreamWriter("App_Cache/build_server_last_status.txt"))
            {
                writer.Write(online ? "online" : "offline");
            }
            return online;
        }
    }
}

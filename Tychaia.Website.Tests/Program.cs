// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Tychaia.Website.ViewModels;
using Argotic.Syndication;
using System.IO;

namespace Tychaia.Website.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var feed = new AtomFeed();
            RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                new FileInfo(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "../../../Tychaia.Website/Views/Home/Index.cshtml")).FullName,
                new FeedViewModel { Feed = feed });
        }
    }
}


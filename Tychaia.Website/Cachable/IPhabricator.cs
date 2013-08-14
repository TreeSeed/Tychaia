// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Runtime.Caching;
using Argotic.Syndication;
using Phabricator.Conduit;
using Tychaia.Website.Models;

namespace Tychaia.Website.Cachable
{
    public interface IPhabricator
    {
        MemoryCache WikiPageCache { get; }
        MemoryCache WikiHierarchyCache { get; }
        MemoryCache BlogCache { get; }
        MemoryCache RemarkupCache { get; }

        void ClearCache();
        string ProcessRemarkup(ConduitClient client, string remarkup);
        dynamic GetWikiPage(ConduitClient client, string slug);
        dynamic GetWikiHierarchy(ConduitClient client, string slug);
        TychaiaTuesdayIssueModel GetTychaiaTuesdayIssue(ConduitClient client, int issue);
        AtomFeed GetFeed(string id);
    }
}


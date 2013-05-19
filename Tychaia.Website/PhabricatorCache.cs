//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Caching;
using Phabricator.Conduit;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace Tychaia.Website
{
    public static class Phabricator
    {
        public static MemoryCache WikiPageCache = new MemoryCache("wiki-page-cache");
        public static MemoryCache WikiHierarchyCache = new MemoryCache("wiki-hierarchy-cache");
        public static MemoryCache BlogCache = new MemoryCache("blog-cache");
        public static MemoryCache RemarkupCache = new MemoryCache("remarkup-cache");

        public static void ClearCache()
        {
            // Switch using temporary variable so that we don't have
            // any threading issues accessing a disposed cache.
            var oldWikiPage = WikiPageCache;
            var oldWikiHierarchy = WikiHierarchyCache;
            var oldBlog = BlogCache;
            var oldRemarkup = RemarkupCache;
            WikiPageCache = new MemoryCache("wiki-page-cache");
            WikiHierarchyCache = new MemoryCache("wiki-hierarchy-cache");
            BlogCache = new MemoryCache("blog-cache");
            RemarkupCache = new MemoryCache("remarkup-cache");
            oldWikiPage.Dispose();
            oldWikiHierarchy.Dispose();
            oldBlog.Dispose();
            oldRemarkup.Dispose();
        }

        private static string SHA1(string input)
        {
            var algorithm = new SHA1Managed();
            return BitConverter.ToString(algorithm.ComputeHash(Encoding.ASCII.GetBytes(input))).Replace("-", string.Empty);
        }

        public static string ProcessRemarkup(ConduitClient client, string remarkup)
        {
            var sha1 = SHA1(remarkup);
            var html = RemarkupCache.Get(sha1) as string;
            if (html == null)
            {
                html = client.Do("remarkup.process", new {
                    context = "phriction",
                    content = remarkup
                }).content;
                Phabricator.RemarkupCache.Add(
                    new CacheItem(sha1, html),
                    new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }
                );
            }
            return html;
        }

        public static dynamic GetWikiPage(ConduitClient client, string slug)
        {
            var page = Phabricator.WikiPageCache.Get(slug);
            if (page == null)
            {
                page = client.Do("phriction.info", new {
                    slug = slug
                });
                Phabricator.WikiPageCache.Add(
                    new CacheItem(slug, page),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }
            return page;
        }

        public static dynamic GetWikiHierarchy(ConduitClient client, string slug)
        {
            var hierarchy = Phabricator.WikiHierarchyCache.Get(slug);
            if (hierarchy == null)
            {
                try
                {
                    hierarchy = client.Do("phriction.hierarchy", new {
                        slug = slug,
                        depth = 2
                    });
                }
                catch (ConduitException)
                {
                    // Ignore this error; Phabricator might not support
                    // the phriction.hierarchy method.
                    hierarchy = new List<object>();
                }
                if (hierarchy == null)
                    throw new NotImplementedException(slug);
                Phabricator.WikiHierarchyCache.Add(
                    new CacheItem(slug, hierarchy),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }
            return hierarchy;
        }
    }
}


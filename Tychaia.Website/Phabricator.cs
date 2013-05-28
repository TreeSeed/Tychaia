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
using Argotic.Syndication;

namespace Tychaia.Website
{
    public class Phabricator : IPhabricator
    {
        public MemoryCache m_WikiPageCache = new MemoryCache("wiki-page-cache");
        public MemoryCache m_WikiHierarchyCache = new MemoryCache("wiki-hierarchy-cache");
        public MemoryCache m_BlogCache = new MemoryCache("blog-cache");
        public MemoryCache m_RemarkupCache = new MemoryCache("remarkup-cache");

        public MemoryCache WikiPageCache { get { return this.m_WikiPageCache; } }
        public MemoryCache WikiHierarchyCache { get { return this.m_WikiHierarchyCache; } }
        public MemoryCache BlogCache { get { return this.m_BlogCache; } }
        public MemoryCache RemarkupCache { get { return this.m_RemarkupCache; } }

        public void ClearCache()
        {
            // Switch using temporary variable so that we don't have
            // any threading issues accessing a disposed cache.
            var oldWikiPage = WikiPageCache;
            var oldWikiHierarchy = WikiHierarchyCache;
            var oldBlog = BlogCache;
            var oldRemarkup = RemarkupCache;
            this.m_WikiPageCache = new MemoryCache("wiki-page-cache");
            this.m_WikiHierarchyCache = new MemoryCache("wiki-hierarchy-cache");
            this.m_BlogCache = new MemoryCache("blog-cache");
            this.m_RemarkupCache = new MemoryCache("remarkup-cache");
            oldWikiPage.Dispose();
            oldWikiHierarchy.Dispose();
            oldBlog.Dispose();
            oldRemarkup.Dispose();
        }

        private static string SHA1(string input)
        {
            var algorithm = new SHA1Managed();
            return BitConverter.ToString(algorithm.ComputeHash(
                Encoding.ASCII.GetBytes(input))).Replace("-", string.Empty);
        }

        public string ProcessRemarkup(ConduitClient client, string remarkup)
        {
            var sha1 = SHA1(remarkup);
            var html = RemarkupCache.Get(sha1) as string;
            if (html == null)
            {
                html = client.Do("remarkup.process", new {
                    context = "phriction",
                    content = remarkup
                }).content;
                this.RemarkupCache.Add(
                    new CacheItem(sha1, html),
                    new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }
                );
            }
            return html;
        }

        public dynamic GetWikiPage(ConduitClient client, string slug)
        {
            var page = this.WikiPageCache.Get(slug);
            if (page == null)
            {
                page = client.Do("phriction.info", new {
                    slug = slug
                });
                this.WikiPageCache.Add(
                    new CacheItem(slug, page),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }
            return page;
        }

        public dynamic GetWikiHierarchy(ConduitClient client, string slug)
        {
            var hierarchy = this.WikiHierarchyCache.Get(slug);
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
                this.WikiHierarchyCache.Add(
                    new CacheItem(slug, hierarchy),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }
            return hierarchy;
        }

        public AtomFeed GetFeed(string id)
        {
            var feed = this.BlogCache.Get("summary-feed-" + id) as AtomFeed;
            if (feed == null)
            {
                feed = AtomFeed.Create(new Uri("http://code.redpointsoftware.com.au/phame/blog/feed/1/"));
                this.BlogCache.Add(
                    new CacheItem("summary-feed-" + id, feed),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }
            return feed;
        }
    }
}


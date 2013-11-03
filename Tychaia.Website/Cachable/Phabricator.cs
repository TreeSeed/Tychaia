// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using Phabricator.Conduit;
using Tychaia.Website.Models;

namespace Tychaia.Website.Cachable
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
                html = client.Do("remarkup.process", new
                {
                    context = "phriction",
                    contents = new string[] { remarkup }
                })[0].content;
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
                page = client.Do("phriction.info", new
                {
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
                    hierarchy = client.Do("phriction.hierarchy", new
                    {
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

        public IEnumerable<BlogPostModel> GetBlogPosts(ConduitClient client)
        {
            var cachedPosts = this.BlogCache.Get("posts");
            if (cachedPosts != null)
                return (IEnumerable<BlogPostModel>)cachedPosts;

            var posts = new List<BlogPostModel>();

            // Get all of the posts.
            var rawPosts = client.Do("phame.queryposts", new
            {
                blogPHIDs = new string[] { "PHID-BLOG-qvobh7al2y7ji3krp6ki", "PHID-BLOG-7n3gn42p5ty2jlfmpedr" }
            });
            foreach (var rawPost in rawPosts)
            {
                posts.Add(new BlogPostModel
                {
                    ID = Convert.ToInt32(rawPost.id),
                    Previous = null,
                    Next = null,
                    Title = rawPost.title,
                    Summary = rawPost.summary,
                    Content = rawPost.body,
                    Author = rawPost.bloggerPHID,
                    Uri = "/blog/" + rawPost.id,
                    UNIXDatePublished = Convert.ToInt64(rawPost.datePublished)
                });
            }
            posts = posts.OrderByDescending(x => x.UNIXDatePublished).ToList();

            // Now process all of the Remarkup fields (Content and Summary).
            var fields = new List<FieldProcessingStruct>();
            foreach (var post in posts)
                fields.Add(new FieldProcessingStruct { Post = post, IsContent = true, Data = post.Content });
            foreach (var post in posts)
                fields.Add(new FieldProcessingStruct { Post = post, IsContent = false, Data = post.Summary });
            var markup = client.Do("remarkup.process", new
            {
                context = "phame",
                contents = fields.Select(x => x.Data).ToArray()
            });
            for (var i = 0; i < fields.Count; i++)
            {
                var m = markup[i];
                var f = fields[i];
                if (f.IsContent)
                    f.Post.Content = m.content;
                else
                    f.Post.Summary = m.content;
            }

            // Now process all of the bloggers.
            var bloggerPHIDs = posts.Select(x => x.Author).ToArray();
            var rawUsers = client.Do("user.query", new
            {
                phids = bloggerPHIDs
            });
            var userMappings = new List<PhabricatorUserMapping>();
            foreach (var rawUser in rawUsers)
            {
                userMappings.Add(new PhabricatorUserMapping
                {
                    PHID = rawUser.phid,
                    RealName = rawUser.realName
                });
            }
            var users = userMappings.ToDictionary(x => x.PHID, x => x.RealName);
            foreach (var post in posts)
            {
                post.Author = users[post.Author];
            }

            // Cache the posts.
            this.BlogCache.Add(
                new CacheItem("posts", posts),
                new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
            );

            // Return the posts.
            return posts;
        }

        private struct FieldProcessingStruct
        {
            public BlogPostModel Post { get; set; }
            public bool IsContent { get; set; }
            public string Data { get; set; }
        }

        private struct PhabricatorUserMapping
        {
            public string PHID { get; set; }
            public string RealName { get; set; }
        }
    }
}


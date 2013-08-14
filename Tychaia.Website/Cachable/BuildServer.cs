// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Runtime.Caching;
using System.Net;

namespace Tychaia.Website.Cachable
{
    public class BuildServer : IBuildServer
    {
        public MemoryCache OnlineStatusCache = new MemoryCache("online-status-cache");

        public void ClearCache()
        {
            // Switch using temporary variable so that we don't have
            // any threading issues accessing a disposed cache.
            var oldOnlineStatus = OnlineStatusCache;
            OnlineStatusCache = new MemoryCache("online-status-cache");
            oldOnlineStatus.Dispose();
        }

        public bool IsBuildServerOnline()
        {
            var online = OnlineStatusCache.Get("online") as bool?;
            if (online == null)
            {
                var client = new WebClient();
                try
                {
                    client.DownloadString("http://build.redpointsoftware.com.au/");
                    online = true;
                }
                catch (Exception)
                {
                    online = false;
                }
                OnlineStatusCache.Add(
                    new CacheItem("online", online),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }
            return online.Value;
        }
    }
}

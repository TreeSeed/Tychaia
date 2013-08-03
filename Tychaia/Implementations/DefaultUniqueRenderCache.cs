//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Globals;

namespace Tychaia
{
    public class DefaultUniqueRenderCache : IUniqueRenderCache
    {
        private IFilteredConsole m_FilteredConsole;
        private Dictionary<long, UniqueRenderPair> m_RenderPairs = new Dictionary<long, UniqueRenderPair>();

        public DefaultUniqueRenderCache(IFilteredConsole filteredConsole)
        {
            this.m_FilteredConsole = filteredConsole;
        }

        public bool Has(int[] data)
        {
            return m_RenderPairs.Keys.Contains(GetHash(data));
        }

        public bool IsWaiting(int[] data)
        {
            long hash = GetHash(data);
            if (m_RenderPairs.Keys.Contains(hash))
                return m_RenderPairs[hash] == null;
            else
                throw new InvalidOperationException();
        }

        public UniqueRender Store(int[] data, RenderTarget2D target, RenderTarget2D depth)
        {
            long hash = GetHash(data);
            UniqueRenderPair urp;
            if (m_RenderPairs.Keys.Contains(hash) && m_RenderPairs[hash] == null /* is waiting */)
            {
                // Remove before new store.
                this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing waiting result.");
                m_RenderPairs.Remove(hash);
            }
            if (m_RenderPairs.Keys.Contains(hash))
            {
                // This is a copy of an existing render, free the passed parameters.
                target.Dispose();
                depth.Dispose();
                urp = m_RenderPairs[hash];
                urp.ReferenceCount += 1;
                this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Storing existing render result (ref+1).");
            }
            else
            {
                urp = new UniqueRenderPair();
                urp.ReferenceCount = (uint)1;
                urp.Target = target;
                urp.DepthMap = depth;
                m_RenderPairs.Add(hash, urp);
                this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Storing new render result (ref=1,mem+1).");
            }
            return new UniqueRender(urp.Target, urp.DepthMap);
        }

        public void StoreWaiting(int[] data)
        {
            long hash = GetHash(data);
            if (m_RenderPairs.Keys.Contains(hash))
                return;
            this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Storing waiting render result.");
            m_RenderPairs.Add(hash, null);
        }
        
        public UniqueRender Grab(int[] data)
        {
            long hash = GetHash(data);
            if (!m_RenderPairs.Keys.Contains(hash))
                throw new InvalidOperationException();
            UniqueRenderPair urp = m_RenderPairs[hash];
            if (urp == null)
                throw new InvalidOperationException();
            urp.ReferenceCount += 1;
            this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Grabbing existing render result (ref+1).");
            return new UniqueRender(urp.Target, urp.DepthMap);
        }

        public void Release(int[] data)
        {
            long hash = GetHash(data);
            if (!m_RenderPairs.Keys.Contains(hash))
                throw new InvalidOperationException();
            UniqueRenderPair urp = m_RenderPairs[hash];
            urp.ReferenceCount -= 1;
            if (urp.ReferenceCount == 0)
            {
                urp.Target.Dispose();
                urp.DepthMap.Dispose();
                m_RenderPairs.Remove(hash);
                this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing and freeing render result (ref-1,mem-1).");
            }
            else
                this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing render result (ref-1).");
        }

        public void ReleaseWaiting(int[] data)
        {
            long hash = GetHash(data);
            if (!m_RenderPairs.Keys.Contains(hash))
                throw new InvalidOperationException();
            UniqueRenderPair urp = m_RenderPairs[hash];
            if (urp != null)
                return;
            m_RenderPairs.Remove(hash);
            this.m_FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing waiting result.");
        }

        private long GetHash(int[] data)
        {
            unchecked
            {
                long hash = 5610979583159;
                for (int i = 0; i < data.Length; i++)
                {
                    hash = hash ^ data[i] ^ i * 6187629050149;
                    hash += 16138338218447;
                    hash = hash ^ data[i] ^ i * 6187629050149;
                    hash += 16138338218447;
                    hash = hash ^ data[i] ^ i * 6187629050149;
                    hash += 16138338218447;
                }
                return hash;
            }
        }

        private class UniqueRenderPair
        {
            public uint ReferenceCount;
            public RenderTarget2D Target;
            public RenderTarget2D DepthMap;
        }
    }
}


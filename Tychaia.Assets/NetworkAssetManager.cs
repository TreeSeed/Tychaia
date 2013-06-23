//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Process4.Attributes;
using System.Collections.Generic;
using Tychaia.Globals;
using Ninject;
using System.Linq;

namespace Tychaia.Assets
{
    /// <summary>
    /// An implementation of an asset manager where the assets can be
    /// modified and changed over the network (usually by the asset
    /// manager program).
    /// </summary>
    [Distributed]
    public class NetworkAssetManager : IAssetManager
    {
        public string Status { get; set; }
        public bool IsRemoting { get { return true; } }

        [ClientCallable]
        public bool IsReady()
        {
            return IoC.Kernel.TryGet<IRawAssetLoader>() != null;
        }

        [Local]
        private IRawAssetLoader m_RawAssetLoader;

        [Local]
        private Dictionary<string, NetworkAsset> m_Assets;

        public NetworkAssetManager()
        {
            this.m_Assets = new Dictionary<string, NetworkAsset>();
        }

        public void Dirty(string asset)
        {
            lock (this.m_Assets)
            {
                this.m_Assets[asset].Dirty = true;
            }
        }

        public IAsset[] GetAll()
        {
            lock (this.m_Assets)
            {
                return this.m_Assets.Values.ToArray();
            }
        }

        [ClientCallable]
        public IAsset Get(string asset)
        {
            lock (this.m_Assets)
            {
                if (this.m_Assets.ContainsKey(asset))
                {
                    if (!this.m_Assets[asset].Dirty)
                        return this.m_Assets[asset];
                    this.m_Assets.Remove(asset);
                }
                if (this.m_RawAssetLoader == null)
                    this.m_RawAssetLoader = IoC.Kernel.Get<IRawAssetLoader>();
                object raw;
                NetworkAsset result;
                try
                {
                    raw = this.m_RawAssetLoader.LoadRawAsset(asset);
                    result = new NetworkAsset(raw, asset, this);
                }
                catch (AssetNotFoundException ex)
                {
                    // Return a network asset which will default automatically (no
                    // loader can match a null asset, so we find the default for
                    // the type when Resolve is finally called on the NetworkAsset).
                    result = new NetworkAsset(null, asset, this);
                }
                this.m_Assets.Add(asset, result);
                return result;
            }
        }
    }
}


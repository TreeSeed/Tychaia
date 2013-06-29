//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Tychaia.Globals;
using System.IO;

namespace Tychaia.Assets
{
    /// <summary>
    /// An implementation of an asset manager that is designed
    /// to edit and build files locally, without the asset
    /// manager program being connected to a running game.
    /// </summary>
    public class LocalAssetManager : IAssetManager
    {
        public string Status { get; set; }
        public bool IsRemoting { get { return false; } }

        private IRawAssetLoader m_RawAssetLoader;
        private Dictionary<string, IAsset> m_Assets = new Dictionary<string, IAsset>();
        private string m_Path;

        public LocalAssetManager(string path)
        {
            this.m_Path = new DirectoryInfo(path).FullName;
        }

        public void Dirty(string asset)
        {
        }

        private void RescanAssets(string prefixes = "")
        {
            var directoryInfo = new DirectoryInfo(this.m_Path + "/" + prefixes.Replace('.', '/'));
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Extension != ".asset")
                    continue;
                var name = file.Name.Substring(0, file.Name.Length - ".asset".Length);
                var asset = (prefixes.Trim('.') + "." + name).Trim('.');
                this.Get(asset);
            }
            foreach (var directory in directoryInfo.GetDirectories())
            {
                this.RescanAssets(prefixes + directory.Name + ".");
            }
        }

        public IAsset Get(string asset)
        {
            if (this.m_Assets.ContainsKey(asset))
                return this.m_Assets[asset];
            if (this.m_RawAssetLoader == null)
                this.m_RawAssetLoader = IoC.Kernel.Get<IRawAssetLoader>();
            var obj = this.m_RawAssetLoader.LoadRawAsset(asset);
            var loaders = IoC.Kernel.GetAll<IAssetLoader>().ToArray();
            if (obj != null)
            {
                foreach (var loader in loaders)
                {
                    var canLoad = false;
                    try
                    {
                        canLoad = loader.CanHandle(obj);
                    }
                    catch (Exception)
                    {
                    }
                    if (canLoad)
                    {
                        var result = loader.Handle(asset, obj);
                        this.m_Assets.Add(asset, result);
                        return result;
                    }
                }
            }
            // NOTE: We don't use asset defaults with the local asset manager, if it
            // doesn't exist, the load fails.
            throw new InvalidOperationException(
                "Unable to load asset '" + asset + "'.  " +
                "No loader for this asset could be found.");
        }

        public IAsset[] GetAll()
        {
            lock (this.m_Assets)
            {
                this.RescanAssets();
                return this.m_Assets.Values.ToArray();
            }
        }
    }
}


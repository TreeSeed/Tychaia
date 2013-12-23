using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;
using System;
using Ninject;

namespace Protogame
{
    /// <summary>
    /// This is the local asset manager for WorldGenViewerAssetManagerProvider. 
    /// </summary>
    public class WorldGenViewerAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public WorldGenViewerAssetManagerProvider(
            IKernel kernel,
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IAssetLoader[] loaders,
            IAssetSaver[] savers,
            ITransparentAssetCompiler transparentAssetCompiler)
        {
            this.m_AssetManager = new LocalAssetManager(
                kernel,
                rawLoader,
                rawSaver,
                loaders,
                savers,
                transparentAssetCompiler);
            this.m_AssetManager.AllowSourceOnly = true;
            this.m_AssetManager.SkipCompilation = true;
        }

        public bool IsReady
        {
            get
            {
                return true;
            }
        }

        public IAssetManager GetAssetManager(bool permitCreate)
        {
            return this.m_AssetManager;
        }
    }
}


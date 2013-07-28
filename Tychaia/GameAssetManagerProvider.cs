//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;
using System;

namespace Tychaia
{
    public class GameAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public GameAssetManagerProvider(
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IEnumerable<IAssetLoader> loaders,
            IEnumerable<IAssetSaver> savers)
        {
            this.m_AssetManager = new LocalAssetManager(
                rawLoader,
                rawSaver,
                loaders,
                savers);
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


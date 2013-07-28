//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Protogame;

namespace Tychaia
{
    public class BlockAsset : MarshalByRefObject, IAsset
    {
        private IAssetManager m_AssetManager;
        private string m_IsometricCubeName;
        private IsometricCubeAsset m_IsometricCube;
    
        public string Name { get; private set; }
        public bool Impassable { get; set; }
        
        public IsometricCubeAsset IsometricCube
        {
            get
            {
                if (this.m_IsometricCube == null)
                    this.m_IsometricCube = this.m_AssetManager.Get<IsometricCubeAsset>(this.m_IsometricCubeName);
                return this.m_IsometricCube;
            }
        }

        public BlockAsset(
            IAssetManager assetManager,
            string name,
            string isometricCubeName,
            bool impassable)
        {
            this.Name = name;
            this.Impassable = impassable;
            this.m_AssetManager = assetManager;
            this.m_IsometricCubeName = isometricCubeName;
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(BlockAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to BlockAsset.");
        }
    }
}


//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Protogame;

namespace Tychaia
{
    public class IsometricCubeAsset : MarshalByRefObject, IAsset
    {
        private IAssetManager m_AssetManager;
        private string m_TopTextureName;
        private string m_LeftTextureName;
        private string m_RightTextureName;
        private TextureAsset m_TopTexture;
        private TextureAsset m_LeftTexture;
        private TextureAsset m_RightTexture;
    
        public string Name { get; private set; }
        
        public TextureAsset TopTexture
        {
            get
            {
                if (this.m_TopTexture == null)
                    this.m_TopTexture = this.m_AssetManager.Get<TextureAsset>(this.m_TopTextureName);
                return this.m_TopTexture;
            }
        }
        
        public TextureAsset LeftTexture
        {
            get
            {
                if (this.m_LeftTexture == null)
                    this.m_LeftTexture = this.m_AssetManager.Get<TextureAsset>(this.m_LeftTextureName);
                return this.m_LeftTexture;
            }
        }
        
        public TextureAsset RightTexture
        {
            get
            {
                if (this.m_RightTexture == null)
                    this.m_RightTexture = this.m_AssetManager.Get<TextureAsset>(this.m_RightTextureName);
                return this.m_RightTexture;
            }
        }

        public IsometricCubeAsset(
            IAssetManager assetManager,
            string name,
            string topTextureName,
            string leftTextureName,
            string rightTextureName)
        {
            this.Name = name;
            this.m_AssetManager = assetManager;
            this.m_TopTextureName = topTextureName;
            this.m_LeftTextureName = leftTextureName;
            this.m_RightTextureName = rightTextureName;
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(IsometricCubeAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to IsometricCubeAsset.");
        }
    }
}


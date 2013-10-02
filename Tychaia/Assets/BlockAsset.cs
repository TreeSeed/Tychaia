// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class BlockAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        #region Texture Fields
        
        private readonly string m_BackTextureName;
        private readonly string m_BottomTextureName;
        private readonly string m_FrontTextureName;
        private readonly string m_LeftTextureName;
        private readonly string m_RightTextureName;
        private readonly string m_TopTextureName;
        private TextureAsset m_BackTexture;
        private TextureAsset m_BottomTexture;
        private TextureAsset m_FrontTexture;
        private TextureAsset m_LeftTexture;
        private TextureAsset m_RightTexture;
        private TextureAsset m_TopTexture;

        #endregion

        public BlockAsset(
            IAssetManager assetManager,
            string name,
            string topTextureName,
            string bottomTextureName,
            string leftTextureName,
            string rightTextureName,
            string frontTextureName,
            string backTextureName,
            bool impassable)
        {
            this.Name = name;
            this.Impassable = impassable;
            this.m_AssetManager = assetManager;
            this.m_TopTextureName = topTextureName;
            this.m_BottomTextureName = bottomTextureName;
            this.m_LeftTextureName = leftTextureName;
            this.m_RightTextureName = rightTextureName;
            this.m_FrontTextureName = frontTextureName;
            this.m_BackTextureName = backTextureName;
        }

        public bool Impassable { get; set; }

        #region Texture Properties

        public TextureAsset TopTexture
        {
            get 
            {
                return this.m_TopTexture ??
                       (this.m_TopTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_TopTextureName));
            }
            
            set
            {
                this.m_TopTexture = value;
            }
        }

        public TextureAsset BottomTexture
        {
            get
            {
                return this.m_BottomTexture ??
                       (this.m_BottomTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_BottomTextureName));
            }
            
            set
            {
                this.m_BottomTexture = value;
            }
        }

        public TextureAsset LeftTexture
        {
            get 
            {
                return this.m_LeftTexture ??
                       (this.m_LeftTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_LeftTextureName));
            }
            
            set
            {
                this.m_LeftTexture = value;
            }
        }

        public TextureAsset RightTexture
        {
            get 
            {
                return this.m_RightTexture ??
                       (this.m_RightTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_RightTextureName));
            }
            
            set
            {
                this.m_RightTexture = value;
            }
        }

        public TextureAsset FrontTexture
        {
            get 
            {
                return this.m_FrontTexture ??
                       (this.m_FrontTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_FrontTextureName));
            }
            
            set
            {
                this.m_FrontTexture = value;
            }
        }

        public TextureAsset BackTexture
        {
            get 
            {
                return this.m_BackTexture ??
                       (this.m_BackTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_BackTextureName));
            }
            
            set
            {
                this.m_BackTexture = value;
            }
        }

        #endregion

        public string Name { get; private set; }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(BlockAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to BlockAsset.");
        }

        public void BuildRenderList(
            TextureAtlasAsset textureAtlasAsset,
            int x,
            int y,
            int z,
            Func<int, int, int, BlockAsset> getRelativeBlock,
            Func<float, float, float, float, float, int> addOrGetVertex,
            Action<int> addIndex)
        {
            var above = getRelativeBlock(0, 1, 0);
            var below = getRelativeBlock(0, -1, 0);
            var east = getRelativeBlock(1, 0, 0);
            var west = getRelativeBlock(-1, 0, 0);
            var north = getRelativeBlock(0, 0, -1);
            var south = getRelativeBlock(0, 0, 1);

            if (above == null)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.TopTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y + 1, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y + 1, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }
            
            if (below == null)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.BottomTexture.Name);
                var topLeft = addOrGetVertex(x, y, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }
            
            if (west == null)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.LeftTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x, y + 1, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x, y, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }
            
            if (east == null)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.RightTexture.Name);
                var topLeft = addOrGetVertex(x + 1, y + 1, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x + 1, y, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }
            
            if (north == null)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.FrontTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y, z, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }
            
            if (south == null)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.FrontTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1, z + 1, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }
        }
    }
}

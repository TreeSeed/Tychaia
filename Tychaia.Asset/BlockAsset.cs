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

namespace Tychaia.Asset
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
            int edges,
            Func<int, int, int, BlockAsset> getRelativeBlock,
            Func<float, float, float, float, float, int> addOrGetVertex,
            Action<int> addIndex)
        {
            var missingAbove = (edges & 0x00001) != 0;
            var missingBelow = (edges & 0x00002) != 0;
            var missingEast = (edges & 0x00004) != 0;
            var missingWest = (edges & 0x00008) != 0;
            var missingNorth = (edges & 0x00010) != 0;
            var missingSouth = (edges & 0x00020) != 0;
            var missingNorthEast = (edges & 0x00040) != 0;
            var missingNorthWest = (edges & 0x00080) != 0;
            var missingSouthEast = (edges & 0x00100) != 0;
            var missingSouthWest = (edges & 0x00200) != 0;
            var missingBelowEast = (edges & 0x00400) != 0;
            var missingBelowWest = (edges & 0x00800) != 0;
            var missingBelowNorth = (edges & 0x01000) != 0;
            var missingBelowSouth = (edges & 0x02000) != 0;
            var missingBelowNorthEast = (edges & 0x04000) != 0;
            var missingBelowNorthWest = (edges & 0x08000) != 0;
            var missingBelowSouthEast = (edges & 0x10000) != 0;
            var missingBelowSouthWest = (edges & 0x20000) != 0;

            var topLeftCorner = 0;
            var topRightCorner = 0;
            var bottomLeftCorner = 0;
            var bottomRightCorner = 0;

            if (missingAbove)
            {
                if (!(missingEast && missingWest))
                {
                    if (missingEast && !missingBelowEast)
                    {
                        topRightCorner = 1;
                        bottomRightCorner = 1;
                    }

                    if (missingWest && !missingBelowWest)
                    {
                        topLeftCorner = 1;
                        bottomLeftCorner = 1;
                    }
                }

                if (!(missingNorth && missingSouth))
                {
                    if (missingNorth && !missingBelowNorth)
                    {
                        topLeftCorner = 1;
                        topRightCorner = 1;
                    }

                    if (missingSouth && !missingBelowSouth)
                    {
                        bottomLeftCorner = 1;
                        bottomRightCorner = 1;
                    }
                }

                if (missingSouthEast && !missingNorthEast && !missingSouthWest && !missingBelowSouthEast)
                {
                    bottomRightCorner = 1;
                }

                if (missingNorthEast && !missingSouthEast && !missingNorthWest && !missingBelowNorthEast)
                {
                    topRightCorner = 1;
                }

                if (missingSouthWest && !missingNorthWest && !missingSouthEast && !missingBelowSouthWest)
                {
                    bottomLeftCorner = 1;
                }

                if (missingNorthWest && !missingSouthWest && !missingNorthEast && !missingBelowNorthWest)
                {
                    topLeftCorner = 1;
                }
            }

            if (missingAbove)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.TopTexture.Name);

                var topLeft = addOrGetVertex(x, y + 1 - topLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - topRightCorner, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y + 1 - bottomLeftCorner, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y + 1 - bottomRightCorner, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }
            
            if (missingBelow)
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
            
            if (missingWest || missingNorthWest || missingSouthWest || (missingSouth && !missingAbove) || (missingNorth && !missingAbove))
            {
                var uv = textureAtlasAsset.GetUVBounds(this.LeftTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - topLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x, y + 1 - bottomLeftCorner, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x, y, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }
            
            if (missingEast || missingNorthEast || missingSouthEast || (missingSouth && !missingAbove) || (missingNorth && !missingAbove))
            {
                var uv = textureAtlasAsset.GetUVBounds(this.RightTexture.Name);
                var topLeft = addOrGetVertex(x + 1, y + 1 - topRightCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - bottomRightCorner, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x + 1, y, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }
            
            if (missingNorth || missingNorthEast || missingNorthWest || (missingEast && !missingAbove) || (missingWest && !missingAbove))
            {
                var uv = textureAtlasAsset.GetUVBounds(this.FrontTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - topLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - topRightCorner, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y, z, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }
            
            if (missingSouth || missingSouthEast || missingSouthWest || (missingEast && !missingAbove) || (missingWest && !missingAbove))
            {
                var uv = textureAtlasAsset.GetUVBounds(this.FrontTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - bottomLeftCorner, z + 1, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - bottomRightCorner, z + 1, uv.X + uv.Width, uv.Y);
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

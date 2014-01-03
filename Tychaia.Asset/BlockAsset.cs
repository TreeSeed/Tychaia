// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;
using Tychaia.Data;

namespace Tychaia.Asset
{
    public class BlockAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

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

        public BlockAsset(
            IAssetManager assetManager, 
            string name, 
            int blockID, 
            string topTextureName, 
            string bottomTextureName, 
            string leftTextureName, 
            string rightTextureName, 
            string frontTextureName, 
            string backTextureName, 
            bool impassable)
        {
            this.Name = name;
            this.BlockID = blockID;
            this.Impassable = impassable;
            this.m_AssetManager = assetManager;
            this.m_TopTextureName = topTextureName;
            this.m_BottomTextureName = bottomTextureName;
            this.m_LeftTextureName = leftTextureName;
            this.m_RightTextureName = rightTextureName;
            this.m_FrontTextureName = frontTextureName;
            this.m_BackTextureName = backTextureName;
        }

        public TextureAsset BackTexture
        {
            get
            {
                return this.m_BackTexture
                       ?? (this.m_BackTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_BackTextureName));
            }

            set
            {
                this.m_BackTexture = value;
            }
        }

        public int BlockID { get; set; }

        public TextureAsset BottomTexture
        {
            get
            {
                return this.m_BottomTexture
                       ?? (this.m_BottomTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_BottomTextureName));
            }

            set
            {
                this.m_BottomTexture = value;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        public TextureAsset FrontTexture
        {
            get
            {
                return this.m_FrontTexture
                       ?? (this.m_FrontTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_FrontTextureName));
            }

            set
            {
                this.m_FrontTexture = value;
            }
        }

        public bool Impassable { get; set; }

        public TextureAsset LeftTexture
        {
            get
            {
                return this.m_LeftTexture
                       ?? (this.m_LeftTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_LeftTextureName));
            }

            set
            {
                this.m_LeftTexture = value;
            }
        }

        public string Name { get; private set; }

        public TextureAsset RightTexture
        {
            get
            {
                return this.m_RightTexture
                       ?? (this.m_RightTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_RightTextureName));
            }

            set
            {
                this.m_RightTexture = value;
            }
        }

        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        public TextureAsset TopTexture
        {
            get
            {
                return this.m_TopTexture
                       ?? (this.m_TopTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_TopTextureName));
            }

            set
            {
                this.m_TopTexture = value;
            }
        }

        public void BuildRenderList(
            TextureAtlasAsset textureAtlasAsset, 
            int x, 
            int y, 
            int z,
            EdgePoint edgePoint, 
            Func<int, int, int, BlockAsset> getRelativeBlock, 
            Func<float, float, float, float, float, int> addOrGetVertex, 
            Action<int> addIndex)
        {
            // TODO: Make this a togglable option on the asset.
            if (this.Name == "block.Water")
            {
                this.BuildWaterRenderList(textureAtlasAsset, x, y, z, addOrGetVertex, addIndex);
                return;
            }

            var topLeftCorner = edgePoint.TopLeftCorner;

            var topRightCorner = edgePoint.TopRightCorner;

            var bottomLeftCorner = edgePoint.BottomLeftCorner;

            var bottomRightCorner = edgePoint.BottomRightCorner;

            var lowerTopLeftCorner = edgePoint.LowerTopLeftCorner;

            var lowerTopRightCorner = edgePoint.LowerTopRightCorner;

            var lowerBottomLeftCorner = edgePoint.LowerBottomLeftCorner;

            var lowerBottomRightCorner = edgePoint.LowerBottomRightCorner;

            // TODO: Make this a togglable option on the asset.
            if (y == 1 && (topLeftCorner != 0 || topRightCorner != 0 || bottomLeftCorner != 0 || bottomRightCorner != 0))
            {
                this.BuildWaterRenderList(textureAtlasAsset, x, y, z, addOrGetVertex, addIndex);
            }

            if (edgePoint.RenderAbove)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.TopTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - topLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - topRightCorner, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y + 1 - bottomLeftCorner, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(
                    x + 1, 
                    y + 1 - bottomRightCorner, 
                    z + 1, 
                    uv.X + uv.Width, 
                    uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }

            if (edgePoint.RenderBelow)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.BottomTexture.Name);
                var topLeft = addOrGetVertex(x, y - lowerTopLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y - lowerTopRightCorner, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y - lowerBottomLeftCorner, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(
                    x + 1, 
                    y - lowerBottomRightCorner, 
                    z + 1, 
                    uv.X + uv.Width, 
                    uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }

            if (edgePoint.RenderWest)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.LeftTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - topLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x, y + 1 - bottomLeftCorner, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y - lowerTopLeftCorner, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x, y - lowerBottomLeftCorner, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }

            if (edgePoint.RenderEast)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.RightTexture.Name);
                var topLeft = addOrGetVertex(x + 1, y + 1 - topRightCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - bottomRightCorner, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x + 1, y - lowerTopRightCorner, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(
                    x + 1, 
                    y - lowerBottomRightCorner, 
                    z + 1, 
                    uv.X + uv.Width, 
                    uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }

            if (edgePoint.RenderNorth)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.FrontTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - topLeftCorner, z, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - topRightCorner, z, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y - lowerTopLeftCorner, z, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(x + 1, y - lowerTopRightCorner, z, uv.X + uv.Width, uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomRight);
                addIndex(topRight);
            }

            if (edgePoint.RenderSouth)
            {
                var uv = textureAtlasAsset.GetUVBounds(this.FrontTexture.Name);
                var topLeft = addOrGetVertex(x, y + 1 - bottomLeftCorner, z + 1, uv.X, uv.Y);
                var topRight = addOrGetVertex(x + 1, y + 1 - bottomRightCorner, z + 1, uv.X + uv.Width, uv.Y);
                var bottomLeft = addOrGetVertex(x, y - lowerBottomLeftCorner, z + 1, uv.X, uv.Y + uv.Height);
                var bottomRight = addOrGetVertex(
                    x + 1, 
                    y - lowerBottomRightCorner, 
                    z + 1, 
                    uv.X + uv.Width, 
                    uv.Y + uv.Height);
                addIndex(topLeft);
                addIndex(topRight);
                addIndex(bottomLeft);
                addIndex(bottomLeft);
                addIndex(topRight);
                addIndex(bottomRight);
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(BlockAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to BlockAsset.");
        }

        private void BuildWaterRenderList(
            TextureAtlasAsset textureAtlasAsset, 
            int x, 
            int y, 
            int z, 
            Func<float, float, float, float, float, int> addOrGetVertex, 
            Action<int> addIndex)
        {
            var uv = textureAtlasAsset.GetUVBounds("texture.Water");
            var topLeft = addOrGetVertex(x, y + 0.75f, z, uv.X, uv.Y);
            var topRight = addOrGetVertex(x + 1, y + 0.75f, z, uv.X + uv.Width, uv.Y);
            var bottomLeft = addOrGetVertex(x, y + 0.75f, z + 1, uv.X, uv.Y + uv.Height);
            var bottomRight = addOrGetVertex(x + 1, y + 0.75f, z + 1, uv.X + uv.Width, uv.Y + uv.Height);
            addIndex(topLeft);
            addIndex(topRight);
            addIndex(bottomLeft);
            addIndex(bottomLeft);
            addIndex(topRight);
            addIndex(bottomRight);
        }
    }
}
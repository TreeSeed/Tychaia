//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Globals;

namespace Tychaia
{
    public class BlockAsset : MarshalByRefObject, IAsset
    {
        private IAssetManager m_AssetManager;
    
        public string Name { get; private set; }
        public bool Impassable { get; set; }
        
        #region Texture Properties 
        
        private string m_TopTextureName;
        private string m_BottomTextureName;
        private string m_LeftTextureName;
        private string m_RightTextureName;
        private string m_FrontTextureName;
        private string m_BackTextureName;
        private TextureAsset m_TopTexture;
        private TextureAsset m_BottomTexture;
        private TextureAsset m_LeftTexture;
        private TextureAsset m_RightTexture;
        private TextureAsset m_FrontTexture;
        private TextureAsset m_BackTexture;
        
        public TextureAsset TopTexture
        {
            get
            {
                if (this.m_TopTexture == null)
                    this.m_TopTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_TopTextureName);
                return this.m_TopTexture;
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
                if (this.m_BottomTexture == null)
                    this.m_BottomTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_BottomTextureName);
                return this.m_BottomTexture;
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
                if (this.m_LeftTexture == null)
                    this.m_LeftTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_LeftTextureName);
                return this.m_LeftTexture;
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
                if (this.m_RightTexture == null)
                    this.m_RightTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_RightTextureName);
                return this.m_RightTexture;
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
                if (this.m_FrontTexture == null)
                    this.m_FrontTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_FrontTextureName);
                return this.m_FrontTexture;
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
                if (this.m_BackTexture == null)
                    this.m_BackTexture = this.m_AssetManager.TryGet<TextureAsset>(this.m_BackTextureName);
                return this.m_BackTexture;
            }
            set
            {
                this.m_BackTexture = value;
            }
        }
        
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
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(BlockAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to BlockAsset.");
        }
        
        public void Render(IRenderContext context, IRenderCache cache, Vector3 position)
        {
            var vertexes = cache.GetOrSet<VertexBuffer>("block.vertexes", () =>
            {
                var buffer = new VertexBuffer(
                    context.GraphicsDevice,
                    VertexPositionTexture.VertexDeclaration,
                    8,
                    BufferUsage.WriteOnly);
                buffer.SetData(
                        new[]
                        {
                            new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
                            new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 1)),
                            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0)),
                            new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(1, 1)),
                            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, 0)),
                            new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(0, 1)),
                            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
                            new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 1)),
                        });
                return buffer;
            });
            var indices = cache.GetOrSet<IndexBuffer>("block.indices", () =>
            {
                var buffer = new IndexBuffer(
                    context.GraphicsDevice,
                    typeof(short),
                    36,
                    BufferUsage.WriteOnly);
                buffer.SetData(
                    new short[]
                    {
                        0, 2, 1, 1, 2, 3,
                        4, 5, 6, 5, 7, 6,
                        0, 4, 6, 0, 6, 2,
                        1, 7, 5, 1, 3, 7,
                        0, 1, 4, 5, 4, 1,
                        6, 3, 2, 7, 3, 6
                    });
                return buffer;
            });
            
            context.EnableTextures();
            context.SetActiveTexture(this.TopTexture.Texture);
            context.GraphicsDevice.Indices = indices;
            context.GraphicsDevice.SetVertexBuffer(vertexes);
            
            context.World = Matrix.CreateTranslation(position);
            
            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                context.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexes.VertexCount,
                    0,
                    indices.IndexCount / 3);
            }
        }
    }
}


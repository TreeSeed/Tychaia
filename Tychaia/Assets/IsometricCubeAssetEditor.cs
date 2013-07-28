//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public class IsometricCubeAssetEditor : AssetEditor<IsometricCubeAsset>
    {
        private TextBox m_TopTextureAssetName;
        private TextBox m_LeftTextureAssetName;
        private TextBox m_RightTextureAssetName;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_TopTextureAssetName = new TextBox { Text = this.m_TopTextureAssetName == null ? null : this.m_TopTextureAssetName.Text };
            this.m_TopTextureAssetName.TextChanged += (sender, e) =>
            {
                this.m_Asset.TopTexture = assetManager.TryGet<TextureAsset>(this.m_TopTextureAssetName.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_LeftTextureAssetName = new TextBox { Text = this.m_LeftTextureAssetName == null ? null : this.m_LeftTextureAssetName.Text };
            this.m_LeftTextureAssetName.TextChanged += (sender, e) =>
            {
                this.m_Asset.LeftTexture = assetManager.TryGet<TextureAsset>(this.m_LeftTextureAssetName.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_RightTextureAssetName = new TextBox { Text = this.m_RightTextureAssetName == null ? null : this.m_RightTextureAssetName.Text };
            this.m_RightTextureAssetName.TextChanged += (sender, e) =>
            {
                this.m_Asset.RightTexture = assetManager.TryGet<TextureAsset>(this.m_RightTextureAssetName.Text);
                assetManager.Save(this.m_Asset);
            };
            
            var form = new Form();
            form.AddControl("Top Texture Name:", this.m_TopTextureAssetName);
            form.AddControl("Left Texture Name:", this.m_LeftTextureAssetName);
            form.AddControl("Right Texture Name:", this.m_RightTextureAssetName);
            
            editorContainer.SetChild(form);
        }

        public override void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        public override void Bake(IAssetManager assetManager)
        {
            assetManager.Bake(this.m_Asset);
        }
    }
}


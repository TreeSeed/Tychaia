// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class BlockAssetEditor : AssetEditor<BlockAsset>
    {
        private TextBox m_BackTextureNameTextBox;
        private TextBox m_BottomTextureNameTextBox;
        private TextBox m_FrontTextureNameTextBox;
        private TextBox m_LeftTextureNameTextBox;
        private TextBox m_RightTextureNameTextBox;
        private TextBox m_TopTextureNameTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_TopTextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.TopTexture == null ? null : this.m_Asset.TopTexture.Name
            };
            this.m_TopTextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.TopTexture = assetManager.TryGet<TextureAsset>(this.m_TopTextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_BottomTextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.BottomTexture == null ? null : this.m_Asset.BottomTexture.Name
            };
            this.m_BottomTextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.BottomTexture = assetManager.TryGet<TextureAsset>(this.m_BottomTextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_LeftTextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.LeftTexture == null ? null : this.m_Asset.LeftTexture.Name
            };
            this.m_LeftTextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.LeftTexture = assetManager.TryGet<TextureAsset>(this.m_LeftTextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_RightTextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.RightTexture == null ? null : this.m_Asset.RightTexture.Name
            };
            this.m_RightTextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.RightTexture = assetManager.TryGet<TextureAsset>(this.m_RightTextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_FrontTextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.FrontTexture == null ? null : this.m_Asset.FrontTexture.Name
            };
            this.m_FrontTextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.FrontTexture = assetManager.TryGet<TextureAsset>(this.m_FrontTextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_BackTextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.BackTexture == null ? null : this.m_Asset.RightTexture.Name
            };
            this.m_BackTextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.BackTexture = assetManager.TryGet<TextureAsset>(this.m_BackTextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };

            var form = new Form();
            form.AddControl("Top Texture Asset Name:", this.m_TopTextureNameTextBox);
            form.AddControl("Bottom Texture Asset Name:", this.m_BottomTextureNameTextBox);
            form.AddControl("Left Texture Asset Name:", this.m_LeftTextureNameTextBox);
            form.AddControl("Right Texture Asset Name:", this.m_RightTextureNameTextBox);
            form.AddControl("Front Texture Asset Name:", this.m_FrontTextureNameTextBox);
            form.AddControl("Back Texture Asset Name:", this.m_BackTextureNameTextBox);

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

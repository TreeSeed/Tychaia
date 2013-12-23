// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class AbilityTypeDefinitionAssetEditor : AssetEditor<AbilityTypeDefinitionAsset>
    {
        private TextBox m_DisplayNameTextBox;
        private TextBox m_DescriptionTextBox;
        private TextBox m_AITextBox;
        private TextBox m_CategoryTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_DisplayNameTextBox = new TextBox
            {
                Text = this.m_Asset.DisplayName == null ? null : this.m_Asset.DisplayName.Name
            };
            this.m_DisplayNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.DisplayName = assetManager.TryGet<LanguageAsset>(this.m_DisplayNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_DescriptionTextBox = new TextBox
            {
                Text = this.m_Asset.Description == null ? null : this.m_Asset.Description.Name
            };
            this.m_DescriptionTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Description = assetManager.TryGet<LanguageAsset>(this.m_DescriptionTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_AITextBox = new TextBox
            {
                Text = this.m_Asset.AI == null ? null : this.m_Asset.AI.Name
            };
            this.m_AITextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.AI = assetManager.TryGet<AIAsset>(this.m_AITextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            this.m_CategoryTextBox = new TextBox
            {
                Text = this.m_Asset.Category
            };
            this.m_CategoryTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Category = this.m_CategoryTextBox.Text;
                assetManager.Save(this.m_Asset);
            };

            var form = new Form();
            form.AddControl("Display Name:", this.m_DisplayNameTextBox);
            form.AddControl("Description:", this.m_DescriptionTextBox);
            form.AddControl("AI:", this.m_AITextBox);
            form.AddControl("Category:", this.m_CategoryTextBox);

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

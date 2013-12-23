// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class ElementDefinitionAssetEditor : AssetEditor<ElementDefinitionAsset>
    {
        private TextBox m_DisplayNameTextBox;

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

            var form = new Form();
            form.AddControl("Display Name:", this.m_DisplayNameTextBox);

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

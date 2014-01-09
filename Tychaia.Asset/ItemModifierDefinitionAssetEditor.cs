// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Asset
{
    public class ItemModifierDefinitionAssetEditor : AssetEditor<ItemModifierDefinitionAsset>
    {
        private TextBox m_DisplayNameTextBox;
        private TextBox m_CategoryTextBox;
        private TextBox m_EffectTextBox;
        private TextBox m_EffectPerLevelTextBox;

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
            this.m_CategoryTextBox = new TextBox
            {
                Text = this.m_Asset.Category.ToString()
            };
            this.m_CategoryTextBox.TextChanged += (sender, e) =>
            {
                foreach (var i in Enum.GetValues(typeof(ItemCategory)))
                {
                    if (i.Equals(this.m_CategoryTextBox.Text))
                    {
                        this.m_Asset.Category = (ItemCategory)i;
                        assetManager.Save(this.m_Asset);
                        break;
                    }
                }
            };
            this.m_EffectTextBox = new TextBox
            {
                Text = this.m_Asset.Effect
            };
            this.m_EffectTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Effect = this.m_EffectTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_EffectPerLevelTextBox = new TextBox
            {
                Text = this.m_Asset.EffectPerLevel
            };
            this.m_EffectPerLevelTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.EffectPerLevel = this.m_EffectPerLevelTextBox.Text;
                assetManager.Save(this.m_Asset);
            };

            var form = new Form();
            form.AddControl("Display Name:", this.m_DisplayNameTextBox);
            form.AddControl("Category:", this.m_CategoryTextBox);
            form.AddControl("Effect:", this.m_EffectTextBox);
            form.AddControl("Effect per level:", this.m_EffectPerLevelTextBox);

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

// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class SpellDefinitionAssetEditor : AssetEditor<SpellDefinitionAsset>
    {
        private TextBox m_DescriptionTextBox;
        private TextBox m_TargetTextBox;
        private TextBox m_TypeTextBox;
        private TextBox m_RangeTextBox;
        private TextBox m_EffectTextBox;
        private TextBox m_EffectPerLevelTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_DescriptionTextBox = new TextBox
            {
                Text = this.m_Asset.Description
            };
            this.m_DescriptionTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Description = this.m_DescriptionTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_TargetTextBox = new TextBox
            {
                Text = this.m_Asset.Target
            };
            this.m_TargetTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Target = this.m_TargetTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_TypeTextBox = new TextBox
            {
                Text = this.m_Asset.Type
            };
            this.m_TypeTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Type = this.m_TypeTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_RangeTextBox = new TextBox
            {
                Text = this.m_Asset.Range
            };
            this.m_RangeTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Range = this.m_RangeTextBox.Text;
                assetManager.Save(this.m_Asset);
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
            form.AddControl("Description:", this.m_DescriptionTextBox);
            form.AddControl("Target:", this.m_TargetTextBox);
            form.AddControl("Type:", this.m_TypeTextBox);
            form.AddControl("Range:", this.m_RangeTextBox);
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

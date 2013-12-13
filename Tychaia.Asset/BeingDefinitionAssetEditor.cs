// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class BeingDefinitionAssetEditor : AssetEditor<BeingDefinitionAsset>
    {
        private TextBox m_TextureNameTextBox;
        private TextBox m_DisplayNameTextBox;
        private TextBox m_DescriptionTextBox;
        private TextBox m_HealthPerLevelTextBox;
        private TextBox m_MovementSpeedTextBox;
        private TextBox m_EnemyTextBox; // TODO: Make this a CheckBox

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_TextureNameTextBox = new TextBox
            {
                Text = this.m_Asset.Texture == null ? null : this.m_Asset.Texture.Name
            };
            this.m_TextureNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Texture = assetManager.TryGet<TextureAsset>(this.m_TextureNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
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
            this.m_HealthPerLevelTextBox = new TextBox
            {
                Text = this.m_Asset.HealthPerLevel
            };
            this.m_HealthPerLevelTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.HealthPerLevel = this.m_HealthPerLevelTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_MovementSpeedTextBox = new TextBox
            {
                Text = this.m_Asset.MovementSpeed
            };
            this.m_MovementSpeedTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.MovementSpeed = this.m_MovementSpeedTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_EnemyTextBox = new TextBox
            {
                Text = this.m_Asset.Enemy ? "True" : "False"
            };
            this.m_EnemyTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Enemy = this.m_EnemyTextBox.Text.ToLower() == "true" ? true : false;
                assetManager.Save(this.m_Asset);
            };

            var form = new Form();
            form.AddControl("Texture Asset Name:", this.m_TextureNameTextBox);
            form.AddControl("Display Name:", this.m_DisplayNameTextBox);
            form.AddControl("Description:", this.m_DescriptionTextBox);
            form.AddControl("Health per level:", this.m_HealthPerLevelTextBox);
            form.AddControl("Movement speed:", this.m_MovementSpeedTextBox);
            form.AddControl("Enemy:", this.m_EnemyTextBox);

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

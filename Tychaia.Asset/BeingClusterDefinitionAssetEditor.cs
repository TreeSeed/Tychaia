// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class BeingClusterDefinitionAssetEditor : AssetEditor<BeingClusterDefinitionAsset>
    {
        private TextBox m_KeywordTextBox;
        private TextBox m_LevelTextBox;
        private TextBox m_EnemyTextBox;
        private TextBox[] m_BeingNameTextBox;
        private TextBox[] m_MinimumTextBox;
        private TextBox[] m_MaximumTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_KeywordTextBox = new TextBox
            {
                Text = this.m_Asset.Keyword
            };
            this.m_KeywordTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Keyword = this.m_KeywordTextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            this.m_LevelTextBox = new TextBox
            {
                Text = this.m_Asset.LevelRequirement.ToString()
            };
            this.m_LevelTextBox.TextChanged += (sender, e) =>
            {
                int levelRequirement = -1;
                if (int.TryParse(this.m_LevelTextBox.Text, out levelRequirement))
                {
                    this.m_Asset.LevelRequirement = levelRequirement;
                }

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
            this.m_BeingNameTextBox[10] = new TextBox();
            this.m_MinimumTextBox[10] = new TextBox();
            this.m_MaximumTextBox[10] = new TextBox();
            for (int i = 0; i < 10; i++)
            {
                this.m_BeingNameTextBox[i].Text = this.m_Asset.BeingDefinitions == null ? null : this.m_Asset.BeingDefinitions[i].Name;
                this.m_BeingNameTextBox[i].TextChanged += (sender, e) =>
                {
                    this.m_Asset.BeingDefinitions[i] = assetManager.TryGet<BeingDefinitionAsset>(this.m_BeingNameTextBox[i].Text);
                    assetManager.Save(this.m_Asset);
                };
                this.m_MinimumTextBox[i].Text = this.m_Asset.Minimum[i].ToString();

                this.m_MinimumTextBox[i].TextChanged += (sender, e) =>
                {
                    if (!int.TryParse(this.m_MinimumTextBox[i].Text, out this.m_Asset.Minimum[i]))
                    {
                        this.m_Asset.Minimum[i] = -1;
                    }

                    assetManager.Save(this.m_Asset);
                };
                this.m_MaximumTextBox[i].Text = this.m_Asset.Maximum[i].ToString();
                this.m_MaximumTextBox[i].TextChanged += (sender, e) =>
                {
                    if (!int.TryParse(this.m_MaximumTextBox[i].Text, out this.m_Asset.Maximum[i]))
                    {
                        this.m_Asset.Maximum[i] = -1;
                    }

                    assetManager.Save(this.m_Asset);
                };
            }

            var form = new Form();
            for (int i = 0; i < 10; i++)
            {
                form.AddControl("Being Definition Asset " + i + ":", this.m_BeingNameTextBox[i]);
                form.AddControl("Minimum Number:", this.m_MinimumTextBox[i]);
                form.AddControl("Maximum Number:", this.m_MaximumTextBox[i]);
            }

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

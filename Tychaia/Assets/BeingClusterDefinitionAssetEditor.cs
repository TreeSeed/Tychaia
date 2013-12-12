// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class BeingClusterDefinitionAssetEditor : AssetEditor<BeingClusterDefinitionAsset>
    {
        private TextBox[] m_BeingNameTextBox;
        private TextBox[] m_MinimumTextBox;
        private TextBox[] m_MaximumTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_BeingNameTextBox[10] = new TextBox();
            this.m_MinimumTextBox[10] = new TextBox();
            this.m_MaximumTextBox[10] = new TextBox();
            for (int i = 0; i < 10; i++)
            {
                this.m_BeingNameTextBox[i].Text = this.m_Asset.BeingDefinitions == null ? null : this.m_Asset.BeingDefinitions[i].Name;
                this.m_MinimumTextBox[i].Text = this.m_Asset.Minimum[i].ToString();
                this.m_MaximumTextBox[i].Text = this.m_Asset.Maximum[i].ToString();
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

//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.Assets;
using Tychaia.UI;

namespace TychaiaAssetManager
{
    public class TextAssetEditor : AssetEditor<TextAsset>
    {
        private TextBox m_TextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_TextBox = new TextBox { Text = this.m_Asset.Value };
            this.m_TextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.Value = this.m_TextBox.Text;
                assetManager.Save(this.m_Asset);
            };
            editorContainer.SetChild(this.m_TextBox);
        }

        public override void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        public override void Bake()
        {
        }
    }
}


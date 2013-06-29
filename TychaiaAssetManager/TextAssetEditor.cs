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
        public override void BuildLayout(SingleContainer editorContainer)
        {
            var label = new Label { Text = this.m_Asset.Value };
            editorContainer.SetChild(label);
        }
    }
}


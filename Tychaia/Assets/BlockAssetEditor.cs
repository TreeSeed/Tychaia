//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public class BlockAssetEditor : AssetEditor<BlockAsset>
    {
        private TextBox m_IsometricCubeNameTextBox;

        public override void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
            this.m_IsometricCubeNameTextBox = new TextBox { Text = this.m_Asset.IsometricCube == null ? null : this.m_Asset.IsometricCube.Name };
            this.m_IsometricCubeNameTextBox.TextChanged += (sender, e) =>
            {
                this.m_Asset.IsometricCube = assetManager.TryGet<IsometricCubeAsset>(this.m_IsometricCubeNameTextBox.Text);
                assetManager.Save(this.m_Asset);
            };
            
            var form = new Form();
            form.AddControl("Isometric Cube Asset Name:", this.m_IsometricCubeNameTextBox);
            
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


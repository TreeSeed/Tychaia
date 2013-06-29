//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.Assets;
using Tychaia.UI;

namespace TychaiaAssetManager
{
    public interface IAssetEditor
    {
        void SetAsset(IAsset asset);
        void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager);
        void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager);
        void Bake();
        Type GetAssetType();
    }
}


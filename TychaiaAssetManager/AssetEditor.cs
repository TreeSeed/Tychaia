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
    public abstract class AssetEditor<T> : IAssetEditor
    {
        protected T m_Asset;

        public void SetAsset(IAsset asset)
        {
            this.m_Asset = (T)asset;
        }

        public Type GetAssetType()
        {
            return typeof(T);
        }

        public abstract void BuildLayout(SingleContainer editorContainer);
    }
}


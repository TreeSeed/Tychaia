//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia.Assets
{
    public interface IAssetManager
    {
        string Status { get; set; }
        bool IsRemoting { get; }

        void Dirty(string asset);
        IAsset Get(string asset);
        IAsset[] GetAll();
    }
}


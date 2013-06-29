//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia.Assets
{
    public class TextAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is TextAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var textAsset = asset as TextAsset;

            return new
            {
                Loader = typeof(TextAssetLoader).FullName,
                Value = textAsset.Value
            };
        }
    }
}


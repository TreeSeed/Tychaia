//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia.Assets
{
    public class TextAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(TextAssetLoader).FullName;
        }

        public IAsset Handle(string name, dynamic data)
        {
            // The text key is the asset name.
            return new TextAsset(name, data.Value);
        }

        public IAsset GetDefault(string name)
        {
            return new TextAsset(name, "Default Text");
        }
    }
}


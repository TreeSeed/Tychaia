//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Process4.Attributes;

namespace Tychaia.Assets
{
    /// <summary>
    /// An implementation of an asset manager where the assets can be
    /// modified and changed over the network (usually by the asset
    /// manager program).
    /// </summary>
    [Distributed]
    public class NetworkAssetManager : IAssetManager
    {
        public string Status { get; set; }
    }
}


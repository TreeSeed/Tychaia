//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Protogame;

namespace Tychaia
{
    public class IsometricCubeAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(IsometricCubeAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new IsometricCubeAsset(
                assetManager,
                name,
                data.TopTextureName,
                data.LeftTextureName,
                data.RightTextureName);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new InvalidOperationException();
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new IsometricCubeAsset(
                assetManager,
                name,
                (string)null,
                (string)null,
                (string)null);
        }
    }
}


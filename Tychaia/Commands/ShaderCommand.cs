// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia
{
    using Protogame;

    public class ShaderCommand : ICommand
    {
        private readonly IAssetManager m_AssetManager;

        public ShaderCommand(IAssetManagerProvider assetManagerProvider)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
        }

        public string[] Names
        {
            get
            {
                return new[] { "shader" };
            }
        }

        public string[] Descriptions
        {
            get
            {
                return new[] { "Reload shaders." };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            this.m_AssetManager.Dirty("effect.Lighting");

            return "Shaders have been reloaded from disk";
        }
    }
}
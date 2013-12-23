// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Protogame;

namespace Tychaia
{
    public class TitleWorld : MenuWorld
    {
        public TitleWorld(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
            : base(twodRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.Title = this.AssetManager.Get<LanguageAsset>("language.TYCHAIA");

            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.SINGLEPLAYER"),
                () =>
                {
                    this.TargetWorld = this.GameContext.CreateWorld<IWorldFactory>(x => x.CreateConnectWorld(true, GetLANIPAddress(), 9091));
                });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.MULTIPLAYER"),
                () =>
                {
                    this.TargetWorld = this.GameContext.CreateWorld<IWorldFactory>(x => x.CreateMultiplayerWorld());
                });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.EXIT"),
                () =>
                {
                    if (this.GameContext != null)
                        this.GameContext.Game.Exit();
                });
        }

        private static IPAddress GetLANIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ips = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
            return ips.Count == 0 ? IPAddress.Loopback : ips[0];
        }
    }
}

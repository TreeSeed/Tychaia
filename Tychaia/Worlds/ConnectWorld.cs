// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Net;
using Dx.Runtime;
using Microsoft.Xna.Framework;
using Ninject;
using Protogame;
using Tychaia.Network;

namespace Tychaia
{
    public class ConnectWorld : MenuWorld
    {
        private readonly IAssetManager m_AssetManager;
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        
        private readonly FontAsset m_DefaultFont;
        private IPAddress m_Address;
        private int m_Port;
        private Action[] m_Actions;
        private int m_ActionStep;
        private string m_Message = string.Empty;
    
        public ConnectWorld(
            IKernel kernel,
            IDxFactory dxFactory,
            ILevelAPI levelAPI,
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
            : base(twodRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_2DRenderUtilities = twodRenderUtilities;
            
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_Address = IPAddress.Loopback;
            this.m_Port = 9091;
            ILocalNode node = null;
            GameState state = null;
            byte[] initial = null;
            var level = levelAPI.NewLevel("test");
            this.m_Actions = new Action[]
            {
                () => this.m_Message = "Setting up kernel...",
                () => TychaiaTCPNetwork.SetupKernel(kernel, false, this.m_Address, this.m_Port),
                () => this.m_Message = "Creating distributed node...",
                () => node = dxFactory.CreateLocalNode(Caching.PushOnChange, Architecture.ServerClient),
                () => this.m_Message = "Binding node to kernel...",
                () => kernel.Bind<ILocalNode>().ToMethod(x => node),
                () => this.m_Message = "Joining network...",
                () => node.Join(null),
                () => this.m_Message = "Retrieving reference to game state...",
                () => state = new Distributed<GameState>(node, GameState.NAME, true),
                () => this.m_Message = "Joining game...",
                () => state.JoinGame(),
                () => this.m_Message = "Retrieving initial game state...",
                () => initial = state.LoadInitialState(),
                () => this.m_Message = "Starting client...",
                () => this.TargetWorld = this.GameContext.CreateWorld<IWorldFactory>(x => x.CreateTychaiaGameWorld(level))
            };
        }
        
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);
            
            if (this.m_ActionStep < this.m_Actions.Length)
            {
                this.m_Actions[this.m_ActionStep++]();
            }
            
            if (this.m_ActionStep >= this.m_Actions.Length && this.TargetWorld == null)
            {
                throw new InvalidOperationException();
            }
        }
        
        public override void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            base.RenderBelow(gameContext, renderContext);

            if (renderContext.Is3DContext)
                return;

            this.m_2DRenderUtilities.RenderText(
                renderContext,
                new Vector2(400, 400),
                this.m_Message,
                this.m_DefaultFont);
        }
    }
}

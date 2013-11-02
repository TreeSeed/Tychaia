// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dx.Runtime;
using Tychaia.Data;
using Tychaia.Game;

namespace Tychaia
{
    [Distributed]
    public class GameState
    {
        public const string NAME = "gamestate";
        
        [Local]
        private readonly List<Synchronised> m_Synchronised;
        
        [Local]
        private object m_Lock = new object();
        
        public GameState()
        {
            this.m_Synchronised = new List<Synchronised>();
        }
        
        [Local]
        public void Update()
        {
            lock (this.m_Lock)
            {
                foreach (var sync in this.m_Synchronised)
                {
                    sync.Update();
                }
            }
        }
        
        [ClientCallable]
        public string InternalMessage(string message)
        {
            if (message == "testupdate")
            {
                var player = this.m_Synchronised.OfType<Player>().First();
                player.X += 50;
                player.Z += 50;
                return "UPD";
            }
            
            return "NOP";
        }
        
        [ClientCallable]
        public void JoinGame()
        {
            lock (this.m_Lock)
            {
                Console.WriteLine("client joined game");
                
                var player = new Player();
                player.Connect((this as ITransparent).Node, "player", true);
                player.Update();
                this.m_Synchronised.Add(player);
            }
        }
        
        [ClientCallable]
        public byte[] LoadInitialState()
        {
            Console.WriteLine("client loading initial state");
            
            // Make a copy so that changes to m_Synchronised don't impact
            // the setup of the initial game state.
            Synchronised[] copy;
            lock (this.m_Lock)
            {
                copy = this.m_Synchronised.ToArray();
            }
            
            var state = new InitialGameState();
            state.Seed = 123456;
            state.EntityNames = new string[copy.Length];
            state.EntityTypes = new string[copy.Length];
            for (var i = 0; i < copy.Length; i++)
            {
                state.EntityNames[i] = copy[i].SynchronisationName;
                state.EntityTypes[i] = copy[i].GetType().AssemblyQualifiedName;
            }
            
            // Serialize and return data.
            using (var memory = new MemoryStream())
            {
                var serializer = new TychaiaDataSerializer();
                serializer.Serialize(memory, state);
                var result = new byte[memory.Position];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(result, 0, result.Length);
                return result;
            }
        }
    }
}

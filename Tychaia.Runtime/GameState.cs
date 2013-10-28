// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
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
        
        public GameState()
        {
            this.m_Synchronised = new List<Synchronised>();
        }
        
        [Local]
        public void Update()
        {
            foreach (var sync in this.m_Synchronised)
            {
                sync.Update();
            }
        }
        
        [ClientCallable]
        public void JoinGame()
        {
            Console.WriteLine("client joined game");
        }
        
        [ClientCallable]
        public byte[] LoadInitialState()
        {
            Console.WriteLine("client loading initial state");
            
            // Make a copy so that changes to m_Synchronised don't impact
            // the setup of the initial game state.
            var copy = this.m_Synchronised.ToArray();
            
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

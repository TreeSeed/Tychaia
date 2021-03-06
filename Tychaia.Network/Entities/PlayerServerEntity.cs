﻿// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using Microsoft.Xna.Framework;
using ProtoBuf;
using Protogame;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class PlayerServerEntity : IServerEntity
    {
        private readonly MxClient m_Client;

        private readonly TychaiaServer m_Server;

        private readonly IServerEntityFactory m_ServerEntityFactory;

        private readonly TychaiaServerWorld m_ServerWorld;

        private readonly ITerrainSurfaceCalculator m_TerrainSurfaceCalculator;

        private readonly int m_UniqueClientIdentifier;

        public PlayerServerEntity(
            ITerrainSurfaceCalculator terrainSurfaceCalculator, 
            IServerEntityFactory serverEntityFactory, 
            TychaiaServer server, 
            TychaiaServerWorld serverWorld, 
            MxClient client, 
            int uniqueClientIdentifier)
        {
            this.m_TerrainSurfaceCalculator = terrainSurfaceCalculator;
            this.m_ServerEntityFactory = serverEntityFactory;
            this.m_Server = server;
            this.m_ServerWorld = serverWorld;
            this.m_Client = client;
            this.m_UniqueClientIdentifier = uniqueClientIdentifier;
        }

        [Obsolete("This needs to be unified with the PlayerEntity some how")]
        public int MovementSpeed
        {
            get
            {
                return 4;
            }
        }

        public string Name { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void ApplyDelta(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                this.X = reader.ReadSingle();
            }

            if (reader.ReadBoolean())
            {
                this.Y = reader.ReadSingle();
            }

            if (reader.ReadBoolean())
            {
                this.Z = reader.ReadSingle();
            }

            if (reader.ReadBoolean())
            {
                this.Name = reader.ReadString();
            }
        }

        public void CalculateDelta(BinaryWriter writer, IServerEntity fromOther)
        {
            const float Tolerance = 0.001f;

            var fromPlayer = (PlayerServerEntity)fromOther;

            writer.Write(Math.Abs(this.X - fromPlayer.X) > Tolerance);
            if (Math.Abs(this.X - fromPlayer.X) > Tolerance)
            {
                writer.Write(this.X);
            }

            writer.Write(Math.Abs(this.Y - fromPlayer.Y) > Tolerance);
            if (Math.Abs(this.Y - fromPlayer.Y) > Tolerance)
            {
                writer.Write(this.Y);
            }

            writer.Write(Math.Abs(this.Z - fromPlayer.Z) > Tolerance);
            if (Math.Abs(this.Z - fromPlayer.Z) > Tolerance)
            {
                writer.Write(this.X);
            }

            writer.Write(this.Name != fromPlayer.Name);
            if (this.Name != fromPlayer.Name)
            {
                writer.Write(this.Name);
            }
        }

        public float? GetSurfaceY(float x, float z)
        {
            return this.m_TerrainSurfaceCalculator.GetSurfaceY(this.m_ServerWorld.Octree, x, z);
        }

        public void MoveInDirection(int directionInDegrees)
        {
            var x = Math.Sin(MathHelper.ToRadians(directionInDegrees - 45)) * this.MovementSpeed;
            var y = -Math.Cos(MathHelper.ToRadians(directionInDegrees - 45)) * this.MovementSpeed;

            // Determine if moving here would require us to move up by more than 32 pixels.
            var targetX = this.GetSurfaceY(this.X + (float)x, this.Z);
            var targetZ = this.GetSurfaceY(this.X, this.Z + (float)y);

            // We calculate X and Z independently so that we can "slide" along the edge of somewhere
            // that the player can't go.  This creates a more natural feel when walking into something
            // that it isn't entirely possible to walk through.

            // If the target returns null, then the chunk hasn't been generated so don't permit
            // the character to move onto it.
            if (targetX != null)
            {
                // If the target height difference and our current height is greater than 32, don't permit
                // the character to move onto it.  This also prevents the character from falling off
                // tall cliffs.
                if (Math.Abs(targetX.Value - this.Y) <= 32)
                {
                    this.X += (float)x;
                }
            }

            // If the target returns null, then the chunk hasn't been generated so don't permit
            // the character to move onto it.
            if (targetZ != null)
            {
                // If the target height difference and our current height is greater than 32, don't permit
                // the character to move onto it.  This also prevents the character from falling off
                // tall cliffs.
                if (Math.Abs(targetZ.Value - this.Y) <= 32)
                {
                    this.Z += (float)y;
                }
            }
        }

        public IServerEntity Snapshot()
        {
            var entity = this.m_ServerEntityFactory.CreatePlayerServerEntity(
                this.m_Server, 
                this.m_ServerWorld, 
                this.m_Client, 
                this.m_UniqueClientIdentifier);
            entity.Name = this.Name;
            entity.X = this.X;
            entity.Y = this.Y;
            entity.Z = this.Z;
            return entity;
        }

        public void Update()
        {
            // Adjust the player's Y position.
            var surfaceY = this.GetSurfaceY(this.X, this.Z);
            if (surfaceY != null)
            {
                this.Y = surfaceY.Value;
            }

            // TODO: Use the delta functions.
            this.m_Server.SendMessage(
                "player update", 
                InMemorySerializer.Serialize(
                    new PlayerServerState
                    {
                        UniqueClientID = this.m_UniqueClientIdentifier, 
                        X = this.X, 
                        Y = this.Y, 
                        Z = this.Z
                    }));
        }

        public void Leave()
        {
            this.m_Server.SendMessage(
                "player leave", 
                InMemorySerializer.Serialize(
                    new PlayerServerState
                    {
                        UniqueClientID = this.m_UniqueClientIdentifier
                    }));
        }

        [ProtoContract]
        public class PlayerServerState
        {
            [ProtoMember(4)]
            public int UniqueClientID { get; set; }

            [ProtoMember(1)]
            public float X { get; set; }

            [ProtoMember(2)]
            public float Y { get; set; }

            [ProtoMember(3)]
            public float Z { get; set; }
        }
    }
}
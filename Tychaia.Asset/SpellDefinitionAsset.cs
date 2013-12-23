// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia.Asset
{
    public class SpellDefinitionAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        public SpellDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string description,
            string target,
            string type,
            string range,
            string effect,
            string effectPerLevel)
        {
            this.Name = name;
            this.m_AssetManager = assetManager;
            this.Description = description;
            this.Target = target;
            this.Type = type;
            this.Range = range;
            this.Effect = effect;
            this.EffectPerLevel = effect;
        }

        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }
        
        // Depending on ability:
        // Auras for example: Target = Area, Type = The type of effect, Element = null, Range = The radius of the area, Effect = The effect of the aura
        public string Name { get; private set; }
        public string Description { get; set; }
        //// Can be: Single, Self, Cone, Area, etc
        public string Target { get; set; }
        //// Can be: Damage, Healing, Armor, Max Health (for auras)
        public string Type { get; set; }
        public string Element { get; set; }
        public string Range { get; set; }
        public string Effect { get; set; }
        public string EffectPerLevel { get; set; }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(SpellDefinitionAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to SpellDefinitionAsset.");
        }
    }
}

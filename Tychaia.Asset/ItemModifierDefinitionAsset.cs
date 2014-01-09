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
    public class ItemModifierDefinitionAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        public ItemModifierDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string nameText,
            ItemCategory category,
            string effect,
            string effectPerLevel)
        {
            this.Name = name;
            this.m_AssetManager = assetManager;
            this.NameText = nameText;
            this.Category = category;
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
        // The in game name such as "<something> Ring".
        public string NameText { get; set; }
        public ItemCategory Category { get; set; }
        
        // The effect (such as increasing lightning resist)
        public string Effect { get; set; }
        public string EffectPerLevel { get; set; }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(SpellDefinitionAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to ItemModifierDefinitionAsset.");
        }
    }
}

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
        
        private readonly string m_DisplayNameLanguageName;
        private LanguageAsset m_DisplayNameLanguage;

        public ItemModifierDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string displayNameLanguageName,
            ItemCategory category,
            string effect,
            string effectPerLevel)
        {
            this.Name = name;
            this.m_AssetManager = assetManager;
            this.m_DisplayNameLanguageName = displayNameLanguageName;
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
        
        public LanguageAsset DisplayName
        {
            get
            {
                return this.m_DisplayNameLanguage ??
                       (this.m_DisplayNameLanguage = this.m_AssetManager.TryGet<LanguageAsset>(this.m_DisplayNameLanguageName));
            }

            set
            {
                this.m_DisplayNameLanguage = value;
            }
        }
        
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

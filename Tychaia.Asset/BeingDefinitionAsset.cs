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
    public class BeingDefinitionAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        #region Asset Fields
        
        private readonly string m_TextureName;
        private readonly string m_DisplayNameLanguageName;
        private readonly string m_DescriptionLanguageName;
        private TextureAsset m_Texture;
        private LanguageAsset m_DisplayNameLanguage;
        private LanguageAsset m_DescriptionLanguage;

        #endregion

        public BeingDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string displayNameLanguageName,
            string descriptionLanguageName,
            string textureName,
            string healthPerLevel,
            string movementSpeed,
            bool enemy)
        {
            this.Name = name;
            this.m_DisplayNameLanguageName = displayNameLanguageName;
            this.m_DescriptionLanguageName = descriptionLanguageName;
            this.m_AssetManager = assetManager;
            this.m_TextureName = textureName;
            this.HealthPerLevel = healthPerLevel;
            this.MovementSpeed = movementSpeed;
            this.Enemy = enemy;
        }

        public bool SourceOnly { get { return false; } }
        public bool CompiledOnly { get { return false; } }

        #region Asset Properties

        public TextureAsset Texture
        {
            get 
            {
                return this.m_Texture ??
                       (this.m_Texture = this.m_AssetManager.TryGet<TextureAsset>(this.m_TextureName));
            }
            
            set
            {
                this.m_Texture = value;
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

        public LanguageAsset Description
        {
            get
            {
                return this.m_DescriptionLanguage ??
                       (this.m_DescriptionLanguage = this.m_AssetManager.TryGet<LanguageAsset>(this.m_DescriptionLanguageName));
            }

            set
            {
                this.m_DescriptionLanguage = value;
            }
        }
        #endregion

        public string Name { get; private set; }
        public string HealthPerLevel { get; set; }
        public string MovementSpeed { get; set; }
        public bool Enemy { get; set; }
        //// public List<SpellDefinitionAsset> Spells { get; set; }
        //// public List<WeaponDefintionAsset> Weapons { get; set; }
        //// This is for both weapons that are droppable (swords, etc) and natural weapons (claws)

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(BeingDefinitionAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to BeingDefinitionAsset.");
        }
    }
}

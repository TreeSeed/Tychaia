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
    public class AbilityTypeDefinitionAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        #region Asset Fields

        private readonly string m_DisplayNameLanguageName;
        private readonly string m_DescriptionLanguageName;
        private readonly string m_AIName;
        private LanguageAsset m_DisplayNameLanguage;
        private LanguageAsset m_DescriptionLanguage;
        private AIAsset m_AI;

        #endregion

        public AbilityTypeDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string displayNameLanguageName,
            string descriptionLanguageName,
            string aiName,
            string category)
        {
            this.Name = name;
            this.m_DisplayNameLanguageName = displayNameLanguageName;
            this.m_DescriptionLanguageName = descriptionLanguageName;
            this.m_AIName = aiName;
            this.Category = category;
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

        #region Asset Properties

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

        public AIAsset AI
        {
            get
            {
                return this.m_AI ??
                    (this.m_AI = this.m_AssetManager.TryGet<AIAsset>(this.m_AIName));
            }

            set
            {
                this.m_AI = value;
            }
        }

        #endregion

        public string Name { get; private set; }
        public string Category { get; set; }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(ElementDefinitionAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to ElementDefinitionAsset.");
        }
    }
}

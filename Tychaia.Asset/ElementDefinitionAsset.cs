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
    public class ElementDefinitionAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        #region Asset Fields

        private readonly string m_DisplayNameLanguageName;
        private LanguageAsset m_DisplayNameLanguage;

        #endregion

        public ElementDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string displayNameLanguageName)
        {
            this.Name = name;
            this.m_DisplayNameLanguageName = displayNameLanguageName;
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

        #endregion

        public string Name { get; private set; }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(ElementDefinitionAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to ElementDefinitionAsset.");
        }
    }
}

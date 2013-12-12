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

namespace Tychaia
{
    public class BeingClusterDefinitionAsset : MarshalByRefObject, IAsset
    {
        private readonly IAssetManager m_AssetManager;

        #region Asset Fields
        private string[] m_BeingDefinitionsName;
        private BeingDefinitionAsset[] m_BeingDefinitions;
        #endregion

        public BeingClusterDefinitionAsset(
            IAssetManager assetManager,
            string name,
            string[] beingDefinitionsName,
            int[] minimum,
            int[] maximum)
        {
            this.Name = name;
            this.m_BeingDefinitionsName = beingDefinitionsName;
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        #region Asset Properties

        public BeingDefinitionAsset[] BeingDefinitions
        {
            get
            {
                if (this.m_BeingDefinitionsName == null)
                    this.m_BeingDefinitionsName = new string[10];

                if (this.m_BeingDefinitions == null)
                {
                    // do look up for each index
                    for (var i = 0; i < this.m_BeingDefinitionsName.Length; i++)
                    {
                        this.m_BeingDefinitions[i] = this.m_AssetManager.TryGet<BeingDefinitionAsset>(this.m_BeingDefinitionsName[i]);
                    }
                }

                return this.m_BeingDefinitions;
            }
        }

        #endregion

        public string Name { get; private set; }
        //// public string Keyword { get; set; }
        public int[] Minimum { get; set; }
        public int[] Maximum { get; set; }
        //// public int[] ClassDefinition { get; set; }
        //// Add this when Classes are implemented.

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(BeingClusterDefinitionAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to BeingDefinitionAsset.");
        }
    }
}

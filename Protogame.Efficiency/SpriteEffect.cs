﻿#region File Description
//-----------------------------------------------------------------------------
// SpriteEffect.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Protogame.Efficiency
{
	/// <summary>
	/// The default effect used by SpriteBatch.
	/// </summary>
	public class SpriteEffect : Effect
	{
        #region Effect Parameters

		EffectParameter matrixParam;

        #endregion

        #region Methods

		/// <summary>
		/// Creates a new SpriteEffect by cloning parameter settings from an existing instance.
		/// </summary>
		public  SpriteEffect (Effect cloneSource)
            : base(cloneSource)
		{
			CacheEffectParameters ();
		}


		/// <summary>
		/// Creates a clone of the current SpriteEffect instance.
		/// </summary>
		public override Effect Clone ()
		{
			return new SpriteEffect (this);
		}


		/// <summary>
		/// Looks up shortcut references to our effect parameters.
		/// </summary>
		void CacheEffectParameters ()
		{
			matrixParam = Parameters ["MatrixTransform"];
		}


		/// <summary>
		/// Lazily computes derived parameter values immediately before applying the effect.
		/// </summary>
		protected override bool OnApply ()
		{
			Viewport viewport = GraphicsDevice.Viewport;

			Matrix projection = Matrix.CreateOrthographicOffCenter (0, viewport.Width, viewport.Height, 0, 0, 1);
			Matrix halfPixelOffset = Matrix.CreateTranslation (-0.5f, -0.5f, 0);

			matrixParam.SetValue (halfPixelOffset * projection);

			// FIXME: Find out what this means??
			return true;
		}


        #endregion
	}
}

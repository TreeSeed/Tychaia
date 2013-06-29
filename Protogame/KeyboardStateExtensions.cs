using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Protogame
{
    public static class KeyboardStateExtensions
    {
        private static Dictionary<Keys, KeyState> m_KeyState;

        public static bool IsKeyPressed(this KeyboardState state, Keys key)
        {
            if (m_KeyState == null)
                m_KeyState = new Dictionary<Keys, KeyState>();
            var oldPressed = KeyState.Up;
            var newPressed = state[key];
            var result = false;
            if (m_KeyState.ContainsKey(key))
                oldPressed = m_KeyState[key];
            if (oldPressed == KeyState.Up && newPressed == KeyState.Down)
                result = true;
            m_KeyState[key] = state[key];
            return result;
        }
    }
}


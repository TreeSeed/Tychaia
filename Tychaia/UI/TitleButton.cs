//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Assets;

namespace Tychaia.UI
{
    class TitleButton
    {
        private static Random m_Random = new Random();
        private TextAsset m_Text;
        private Rectangle m_Area;
        private Action m_OnClick;
        private double m_PulseValue;
        private bool m_PulseModeUp;
        private bool m_IsDown;

        public TitleButton(TextAsset text, Rectangle area, Action onClick)
        {
            this.m_Text = text;
            this.m_Area = area;
            this.m_OnClick = onClick;
            this.m_PulseValue = m_Random.NextDouble();
            this.m_IsDown = false;
        }

        public int X
        {
            get { return this.m_Area.X; }
            set { this.m_Area.X = value; }
        }

        public int Y
        {
            get { return this.m_Area.Y; }
            set { this.m_Area.Y = value; }
        }

        public void Process(XnaGraphics xna, MouseState mouse)
        {
            if (this.m_PulseValue >= 1)
                this.m_PulseModeUp = false;
            else if (this.m_PulseValue <= 0)
                this.m_PulseModeUp = true;
            this.m_PulseValue += this.m_PulseModeUp ? 0.01 : -0.01;
            if (this.m_Area.Contains(mouse.X, mouse.Y))
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                    this.m_IsDown = true;
                if (this.m_IsDown && mouse.LeftButton != ButtonState.Pressed)
                {
                    this.m_OnClick();
                    this.m_IsDown = false;
                }
            }
            if (this.m_Area.Contains(mouse.X, mouse.Y))
                xna.FillRectangle(
                    this.m_Area,
                    new Color(1f, 1f, 1f, 0.25f + (float)(this.m_PulseValue / 2.0)).ToPremultiplied());
            else
                xna.FillRectangle(
                    this.m_Area,
                    new Color(1f, 1f, 1f, 0.1f + (float)(this.m_PulseValue / 32.0)).ToPremultiplied());
            xna.DrawStringCentered(
                this.m_Area.X + this.m_Area.Width / 2,
                this.m_Area.Y + 4,
                this.m_Text.Value,
                "ButtonFont");
        }
    }
}

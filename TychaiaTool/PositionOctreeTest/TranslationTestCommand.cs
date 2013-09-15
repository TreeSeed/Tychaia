// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using ManyConsole;
using Protogame;
using TychaiaTool.PositionOctreeTest;
using Tychaia.Globals;

namespace TychaiaTool
{
    public class TranslationTestCommand : ConsoleCommand
    {
        private readonly IPositionScaleTranslation m_PositionScaleTranslation;

        public TranslationTestCommand(
            IPositionScaleTranslation positionScaleTranslation)
        {
            this.IsCommand("test-position-translation", "Output various translations");

            this.m_PositionScaleTranslation = positionScaleTranslation;
        }

        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("-1025 ");
            Console.WriteLine(this.m_PositionScaleTranslation.Translate(-1025));
            Console.WriteLine("-1024 ");
            Console.WriteLine(this.m_PositionScaleTranslation.Translate(-1024));
            Console.WriteLine("-1023 ");
            Console.WriteLine(this.m_PositionScaleTranslation.Translate(-1023));

            return 0;
        }
    }
}


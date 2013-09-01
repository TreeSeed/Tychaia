// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ManyConsole;
using System;

namespace TychaiaTool
{
    public interface IConfigurationHelper
    {
        void Setup(ConsoleCommand command, Action<string> assign);
        IProceduralConfiguration Validate(string assigned);
    }
}


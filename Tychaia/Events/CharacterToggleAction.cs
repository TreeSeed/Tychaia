// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class CharacterToggleAction : IEventAction
    {
        public void Handle(IGameContext gameContext, Event @event)
        {
            System.Console.WriteLine("character toggle pressed");
        }
    }
}


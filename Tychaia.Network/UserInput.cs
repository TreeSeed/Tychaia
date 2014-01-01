// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Network
{
    public enum UserInputAction
    {
        Nothing = 0, 

        Move = 1, 
    }

    [ProtoContract]
    public class UserInput
    {
        [ProtoMember(1)]
        public int Action { get; set; }

        [ProtoMember(2)]
        public int DirectionInDegrees { get; set; }

        public UserInputAction GetAction()
        {
            return (UserInputAction)this.Action;
        }

        public void SetAction(UserInputAction action)
        {
            this.Action = (int)action;
        }
    }
}
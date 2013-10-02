// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia
{
    public interface IWorldFactory
    {
        TychaiaGameWorld CreateTychaiaGameWorld(ILevel level);
        PregenerateWorld CreatePregenerateWorld(ILevel level);
    }
}

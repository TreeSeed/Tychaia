// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Globals
{
    public interface IRenderingLocationProvider
    {
        long X { get; }
        long Y { get; }
        long Z { get; }
    }
}

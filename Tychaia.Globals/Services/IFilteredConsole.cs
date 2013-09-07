// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Globals
{
    public interface IFilteredConsole
    {
        void Write(FilterCategory category, string message);
        void WriteLine(FilterCategory category, string message);
    }
}

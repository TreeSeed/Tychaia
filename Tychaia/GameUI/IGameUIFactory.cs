// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia
{
    public interface IGameUIFactory
    {
        InventoryUIEntity CreateInventoryUIEntity();
        StatusBar CreateStatusBar();
        LeftBar CreateLeftBar();
        RightBar CreateRightBar();
        InventoryManager CreateInventoryManager();
    }
}

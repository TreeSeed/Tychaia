// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;

namespace Tychaia.Globals
{
    public interface IPersistentStorage
    {
        /// <summary>
        /// The persistent setting storage.
        /// </summary>
        /// <value>Persistent settings.</value>
        dynamic Settings { get; }

        /// <summary>
        /// The directory for save data to be stored in.
        /// </summary>
        /// <value>The save directory.</value>
        DirectoryInfo SaveDirectory { get; }
    }
}


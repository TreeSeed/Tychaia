using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Disk
{
    public interface ILevelAPI
    {
        /// <summary>
        /// Gets a list of available levels that can be loaded.
        /// </summary>
        /// <returns>A list of available levels.</returns>
        List<string> GetAvailableLevels();

        /// <summary>
        /// Returns a reference to a new level.  This function will also
        /// create any initial data for the level; it does not have to have
        /// Save() called on it for the initial state to be created.
        /// </summary>
        /// <param name="name">The level name.</param>
        /// <returns>The level reference.</returns>
        ILevel NewLevel(string name);

        /// <summary>
        /// Returns a reference to a level stored on disk.
        /// </summary>
        /// <param name="name">The level name.</param>
        /// <returns>The level reference.</returns>
        ILevel LoadLevel(string name);
    }
}

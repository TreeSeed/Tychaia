// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class SaveCommand : ICommand
    {
        private readonly IPersistentStorage m_PersistentStorage;

        public SaveCommand(
            IPersistentStorage persistentStorage)
        {
            this.m_PersistentStorage = persistentStorage;
        }

        public string[] Names
        {
            get
            {
                return new[] { "save" };
            }
        }
        
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Configure the save engine."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length < 1)
                return "Not enough parameters.";

            switch (parameters[0].ToLower())
            {
                case "help":
                    return @"ignore - Configure whether to ignore saved chunks.";
                case "ignore":
                    if (parameters.Length < 2)
                        return "Not enough parameters.";
                    switch (parameters[1].ToLower())
                    {
                        case "help":
                            return @"enable - Prevent chunks from being loaded from disk.
disable - Allow chunks to load from disk (default).
status - Show the status of saved chunks.";
                        case "enable":
                            this.m_PersistentStorage.Settings.IgnoreSavedChunks = true;
                            return "Saved chunks are now ignored.";
                        case "disable":
                            this.m_PersistentStorage.Settings.IgnoreSavedChunks = false;
                            return "Saves chunks will be loaded from disk where available.";
                        case "status":
                            return (this.m_PersistentStorage.Settings.IgnoreSavedChunks ?? true)
                                ? "Saved chunks are ignored."
                                : "Saves chunks will be loaded.";
                        default:
                            return "Unknown command (try `per-method help`).";
                    }
                    
                default:
                    return "Unknown command (try `help`).";
            }
        }
    }
}

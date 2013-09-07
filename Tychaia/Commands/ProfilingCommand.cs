// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class ProfilingCommand : ICommand
    {
        private readonly IPersistentStorage m_PersistentStorage;

        public string[] Names { get { return new[] { "profiler" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Configure the profiler."
                };
            }
        }

        public ProfilingCommand(
            IPersistentStorage persistentStorage)
        {
            this.m_PersistentStorage = persistentStorage;
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length < 1)
                return "Not enough parameters.";

            switch (parameters[0].ToLower())
            {
                case "help":
                    return @"per-method - Configure per-method profiling.";
                case "per-method":
                    if (parameters.Length < 2)
                        return "Not enough parameters.";
                    switch (parameters[1].ToLower())
                    {
                        case "help":
                            return @"enable - Enable per-method profiling.
disable - Disable per-method profiling.
status - Show the status of per-method profiling.";
                        case "enable":
                            this.m_PersistentStorage.Settings.PerMethodProfiling = true;
                            return "Per-method profiling is now enabled.  " +
                                   "Restart the game for changes to take effect.";
                        case "disable":
                            this.m_PersistentStorage.Settings.PerMethodProfiling = false;
                            return "Per-method profiling is now disabled.  " +
                                   "Restart the game for changes to take effect.";
                        case "status":
                            return (this.m_PersistentStorage.Settings.PerMethodProfiling ?? true)
                                ? "Per-method profiling is enabled (changes take effect after game restart)."
                                : "Per-method profiling is disabled (changes take effect after game restart).";
                        default:
                            return "Unknown command (try `per-method help`).";
                    }
                default:
                    return "Unknown command (try `help`).";
            }
        }
    }
}


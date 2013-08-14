// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;
using System.Linq;
using System.ComponentModel;

namespace Tychaia
{
    public class ChunkAICommand : ICommand
    {
        public string[] Names { get { return new[] { "chunk-ai" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Configure chunk AI."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            var world = gameContext.World as TychaiaGameWorld;
            if (world == null)
                return "Not currently in-game.";
            var chunkManagerEntity = (ChunkManagerEntity)world.Entities.FirstOrDefault(x => x is ChunkManagerEntity);
            if (chunkManagerEntity == null)
                return "No current chunk manager entity.";

            var ais = chunkManagerEntity.GetAIs();
            if (parameters.Length < 1)
                return "AI name required.  Pick from: " +
                    (ais.Count() == 0
                        ? ""
                        : ais.Select(x => "\n- " + x.GetType().Name)
                            .Aggregate((current, next) => current + next));

            if (parameters[0] == "help")
                return "chunk-ai <ai-name> <property> [<value>]";

            var ai = ais.FirstOrDefault(x => x.GetType().Name == parameters[0]);
            if (ai == null)
                return "No such AI.  Pick from: " +
                    (ais.Count() == 0
                        ? ""
                        : ais.Select(x => "\n- " + x.GetType().Name)
                            .Aggregate((current, next) => current + next));

            var props = TypeDescriptor.GetProperties(ai).Cast<PropertyDescriptor>().ToArray();
            if (parameters.Length < 2)
                return "Property name required.  Pick from: " +
                    (props.Count() == 0
                        ? ""
                        : props.Select(x => "\n- " + x.Name)
                            .Aggregate((current, next) => current + next));

            var prop = props.FirstOrDefault(x => x.Name == parameters[1]);
            if (prop == null)
                return "No such property.  Pick from: " +
                    (props.Count() == 0
                        ? ""
                        : props.Select(x => "\n- " + x.Name)
                            .Aggregate((current, next) => current + next));

            if (parameters.Length == 3)
            {
                prop.SetValue(ai, parameters[2]);
                return prop.GetValue(ai).ToString();
            }
            else
                return prop.GetValue(ai).ToString();
        }
    }
}


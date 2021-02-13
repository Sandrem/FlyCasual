using System.Collections.Generic;

namespace CommandsList
{
    public class DebugCommand : GenericCommand
    {
        public DebugCommand()
        {
            Keyword = "debug";
            Description = "Shows debug menu";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            UI.ToggleDebugMenu();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class HelpCommand : GenericCommand
    {
        public HelpCommand()
        {
            Keyword = "help";
            Description =   "Shows help for commands\n" +
                            "help <command> - show description of command";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.Count > 0)
            {
                ShowHelpForCommand(parameters.First().Key);
            }
            else
            {
                ShowGeneralHelp();
            }
        }

        private void ShowGeneralHelp()
        {
            Console.Write("\nAvailable commands:", LogTypes.Everything, true);
            foreach (var command in Console.AvailableCommands)
            {
                Console.Write(command.Key);
            }
            Console.Write("\nhelp <command> - show description of command", LogTypes.Everything, true);
        }

        private void ShowHelpForCommand(string command)
        {
            if (Console.AvailableCommands.ContainsKey(command))
            {
                Console.Write("\n" + Console.AvailableCommands[command].Description);
            }
            else
            {
                ShowGeneralHelp();
            }
        }
    }
}

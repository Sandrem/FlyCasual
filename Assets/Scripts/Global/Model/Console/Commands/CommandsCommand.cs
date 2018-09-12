using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class CommandsCommand : GenericCommand
    {
        public CommandsCommand()
        {
            Keyword = "commands";
            Description = "commands show - show list of received commands";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("show"))
            {
                Console.Write("Received commands:");
                foreach (var command in GameController.CommandsReceived)
                {
                    Console.Write(command.ToString());
                }
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

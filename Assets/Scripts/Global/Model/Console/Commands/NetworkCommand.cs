using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class NetworkCommand : GenericCommand
    {
        public NetworkCommand()
        {
            Keyword = "network";
            Description = "network chat t:<text> - send chat message";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("chat"))
            {
                Network.SendChatMessage(parameters["t"]);
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

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
            Description = "network chat t:<text> - send chat message\n" +
                "network server tcp4://127.0.0.1 - set server uri";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("chat"))
            {
                Network.SendChatMessage(parameters["t"]);
            }
            if (parameters.ContainsKey("server"))
            {
                Network.ServerUri = "tcp4:" + parameters["tcp4"];
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

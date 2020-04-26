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
            Description = "network chatmessage text:<text> - send chat message";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("chatmessage"))
            {
                Network.SendChatMessage(parameters["text"]);
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

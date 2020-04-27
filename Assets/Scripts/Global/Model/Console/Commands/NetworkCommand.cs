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
                "network squads - prepare squads\n" +
                "network start - start network game\n" +
                "network ready - ready network game";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("chat"))
            {
                Network.SendChatMessage(parameters["t"]);
            }
            if (parameters.ContainsKey("squads"))
            {
                Network.SyncSquads();
            }
            if (parameters.ContainsKey("start"))
            {
                Network.StartNetworkGame();
            }
            if (parameters.ContainsKey("ready"))
            {
                Network.BattleIsReady();
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

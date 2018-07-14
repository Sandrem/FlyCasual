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
            Description = "network finish - finish current network task";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("finish"))
            {
                Network.FinishTask();
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

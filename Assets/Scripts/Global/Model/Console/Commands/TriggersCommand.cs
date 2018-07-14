using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class TriggersCommand : GenericCommand
    {
        public TriggersCommand()
        {
            Keyword = "triggers";
            Description = "triggers finish - finish current trigger";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("finish"))
            {
                Triggers.FinishTrigger();
            }
            else
            {
                ShowHelp();
            }
        }
    }
}

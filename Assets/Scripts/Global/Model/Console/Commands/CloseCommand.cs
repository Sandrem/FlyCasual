using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class CloseCommand : GenericCommand
    {
        public CloseCommand()
        {
            Keyword = "close";
            Description = "Closes console window";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            Console.ToggleConsole();
        }
    }
}

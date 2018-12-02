using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class InputCommand : GenericCommand
    {
        public InputCommand()
        {
            Keyword = "input";
            Description =   "Turns on/off input in UI\n" +
                            "Available commands:\n" +
                            "input axis\n" +
                            "input mouse\n";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("axis"))
            {
                CameraScript.InputAxisAreDisabled = !CameraScript.InputAxisAreDisabled;
            }
            else if (parameters.ContainsKey("mouse"))
            {
                CameraScript.InputMouseIsDisabled = !CameraScript.InputMouseIsDisabled;
            }
        }
    }
}

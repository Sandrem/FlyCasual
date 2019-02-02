﻿using System;
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
                "Available commands (mouse and touch controls are exclusive):\n" +
                            "input axis\n" +
                            "input mouse\n" +
                            "input touch\n";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("axis"))
            {
                CameraScript.InputAxisAreEnabled = !CameraScript.InputAxisAreEnabled;
            }
            else if (parameters.ContainsKey("mouse"))
            {
                CameraScript.InputMouseIsEnabled = !CameraScript.InputMouseIsEnabled;
            }
            else if (parameters.ContainsKey("touch")) {
                CameraScript.InputTouchIsEnabled = !CameraScript.InputTouchIsEnabled;
            }
        }
    }
}

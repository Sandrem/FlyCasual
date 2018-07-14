using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CommandsList
{
    public class CopyCommand : GenericCommand
    {
        public CopyCommand()
        {
            Keyword = "copy";
            Description = "Copies all content of log to clipboard";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            string allLog = "";
            foreach (var logEntry in Console.Logs)
            {
                allLog += logEntry.Text;
            }
            GUIUtility.systemCopyBuffer = allLog;

            Console.Write("\nLogs are copied to clipboard");
        }
    }
}

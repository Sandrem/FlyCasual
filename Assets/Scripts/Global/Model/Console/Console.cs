using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;
using System;

public enum LogTypes
{
    Everything,
    Errors,
    Triggers,
    AI,
    Network
}

public partial class Console : MonoBehaviour {

    public class LogEntry
    {
        public string Text;
        public LogTypes Type;
        public float CalculatedPrefferedHeight;

        public LogEntry(string text, LogTypes logType)
        {
            Text = text;
            Type = logType;
        }
    }

    private static List<LogEntry> logs;
    public static List<LogEntry> Logs
    {
        get { return logs; }
        private set { logs = value; }
    }

    private static LogTypes currentLogTypeToShow;
    private static List<LogTypes> logsList = new List<LogTypes>() { LogTypes.Everything, LogTypes.Errors, LogTypes.Triggers, LogTypes.AI, LogTypes.Network };

    private static Dictionary<string, GenericCommand> availableCommands;
    public static Dictionary<string, GenericCommand> AvailableCommands
    {
        get { return availableCommands; }
        private set { availableCommands = value; }
    }


    private void Start()
    {
        Application.logMessageReceived += ProcessUnityLog;

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        AvailableCommands = new Dictionary<string, GenericCommand>();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "CommandsList", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            if (type.MemberType == MemberTypes.NestedType) continue;
            GenericCommand newCommand = (GenericCommand)System.Activator.CreateInstance(type);
        }

        AvailableCommands = AvailableCommands.OrderBy(n => n.Key).ToDictionary(n => n.Key, n => n.Value);
    }

    private static void InitializeLogs()
    {
        Logs = new List<LogEntry>();
        currentLogTypeToShow = LogTypes.Everything;
    }

    public static void Write(string text, LogTypes logType = LogTypes.Everything, bool isBold = false, string color = "")
    {
        if (Logs == null) InitializeLogs();

        string logString = text;
        if (isBold) logString = "<b>" + logString + "</b>";
        if (color != "") logString = "<color="+ color + ">" + logString + "</color>";

        LogEntry logEntry = new LogEntry(logString + "\n", logType);
        Logs.Add(logEntry);

        if (currentLogTypeToShow == logType || currentLogTypeToShow == LogTypes.Everything)
        {
            ShowLogEntry(logEntry);
        }
    }

    private void ProcessUnityLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            if (IsHiddenError(logString)) return;

            IsActive = true;
            Write("\n" + logString + "\n\n" + stackTrace, LogTypes.Errors, true, "red");
        }
    }

    private bool IsHiddenError(string text)
    {
        if ((text == "ClientDisconnected due to error: Timeout") || (text == "ServerDisconnected due to error: Timeout")) return true;

        return false;
    }

    public static void ProcessCommand(string inputText)
    {
        if (string.IsNullOrEmpty(inputText)) return;

        List<string> blocks = inputText.ToLower().Split(' ').ToList();
        string keyword = blocks.FirstOrDefault();
        blocks.RemoveAt(0);

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        foreach (var item in blocks)
        {
            string[] paramValue = item.Split(':');
            if (paramValue.Length == 2) parameters.Add(paramValue[0], paramValue[1]);
            else if (paramValue.Length == 1) parameters.Add(paramValue[0], null);
        }

        if (AvailableCommands.ContainsKey(keyword))
        {
            AvailableCommands[keyword].Execute(parameters);
        }
        else
        {
            Console.Write("Unknown command, enter \"help\" to see list of commands", LogTypes.Everything, false, "red");
        }
    }

    public static void AddAvailableCommand(GenericCommand command)
    {
        AvailableCommands.Add(command.Keyword, command);
    }

}

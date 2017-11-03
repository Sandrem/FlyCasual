using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum LogTypes
{
    Everything,
    Errors,
    Triggers
}

public partial class Console : MonoBehaviour {

    private class LogEntry
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

    private static LogTypes currentLogTypeToShow;

    private static List<LogEntry> logs;

    private void Start()
    {
        Application.logMessageReceived += ProcessUnityLog;
    }

    private static void InitializeLogs()
    {
        logs = new List<LogEntry>();
        currentLogTypeToShow = LogTypes.Everything;
    }

    public static void Write(string text, LogTypes logType = LogTypes.Everything, bool isBold = false, string color = "")
    {
        if (logs == null) InitializeLogs();

        string logString = text;
        if (isBold) logString = "<b>" + logString + "</b>";
        if (color != "") logString = "<color="+ color + ">" + logString + "</color>";

        LogEntry logEntry = new LogEntry(logString + "\n", logType);
        logs.Add(logEntry);

        if (currentLogTypeToShow == logType || currentLogTypeToShow == LogTypes.Everything)
        {
            ShowLogEntry(logEntry);
        }
    }

    private void ProcessUnityLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            IsActive = true;
            Write("\n" + logString + "\n\n" + stackTrace, LogTypes.Errors, true, "red");
        }
    }

    private void ProcessCommand(string inputText)
    {
        if (inputText.ToLower() == "finishtrigger") Triggers.FinishTrigger();
        else if (inputText.ToLower() == "finishsubphase") Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
        else Console.Write("Unknown command", LogTypes.Everything, false, "red");
    }
}

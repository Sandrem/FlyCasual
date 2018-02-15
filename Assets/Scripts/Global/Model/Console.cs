using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum LogTypes
{
    Everything,
    Errors,
    Triggers,
    AI,
    Network
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

    private static List<LogEntry> logs;
    private static LogTypes currentLogTypeToShow;
    private static List<LogTypes> logsList = new List<LogTypes>() { LogTypes.Everything, LogTypes.Errors, LogTypes.Triggers, LogTypes.AI, LogTypes.Network };

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
        if (inputText.ToLower() == "help")
        {
            Console.Write("\nAvailable commands:", LogTypes.Everything, true);
            Console.Write("Multiplayer", LogTypes.Everything);
            Console.Write("Copy", LogTypes.Everything);
            Console.Write("FinishTrigger", LogTypes.Everything);
            Console.Write("FinishSubPhase", LogTypes.Everything);
            Console.Write("FinishNetworkTask", LogTypes.Everything);
            Console.Write("Subphase", LogTypes.Everything);
            Console.Write("Trigger", LogTypes.Everything);
            Console.Write("Decks", LogTypes.Everything);
            Console.Write("Close", LogTypes.Everything);
        }
        else if (inputText.ToLower() == "copy") CopyToClipboard();
        else if (inputText.ToLower() == "finishtrigger") Triggers.FinishTrigger();
        else if (inputText.ToLower() == "finishsubphase") Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
        else if (inputText.ToLower() == "finishnetworktask") Network.FinishTask();
        else if (inputText.ToLower() == "multiplayer") Network.EnableNetwork();
        else if (inputText.ToLower() == "subphase") CurrentSubphase();
        else if (inputText.ToLower() == "decks") DecksContent();
        else if (inputText.ToLower() == "close") ToggleConsole();
        else if (!string.IsNullOrEmpty(inputText)) Console.Write("Unknown command", LogTypes.Everything, false, "red");
    }

    private void CopyToClipboard()
    {
        string allLog = "";
        foreach (var logEntry in logs)
        {
            allLog += logEntry.Text;
        }
        GUIUtility.systemCopyBuffer = allLog;

        Console.Write("Logs are copied to clipboard");
    }

    private void CurrentSubphase()
    {
        Console.Write("Current subphase: " + Phases.CurrentSubPhase.GetType().ToString());
    }

    private void DecksContent()
    {
        Console.Write("Player1 Deck:", LogTypes.Everything, true, "green");
        foreach (var card in DamageDecks.GetDamageDeck(Players.PlayerNo.Player1).Deck)
        {
            Console.Write(card.Name, LogTypes.Everything, false, "green");
        }
        Console.Write("Player2 Deck:", LogTypes.Everything, true, "green");
        foreach (var card in DamageDecks.GetDamageDeck(Players.PlayerNo.Player2).Deck)
        {
            Console.Write(card.Name, LogTypes.Everything, false, "green");
        }
    }

}

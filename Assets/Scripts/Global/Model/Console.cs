using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LogTypes
{
    Everything,
    Errors
}

public class Console : MonoBehaviour {

    private static LogTypes currentLogTypeToShow;

    private static Dictionary<LogTypes, string> logs;

    private static string logEverything = "\n";
    private static string logErrors = "\n";

    private static GameObject ConsoleWindow;
    private static Text ConsoleOutput;

    public static bool IsActive
    {
        get { return ConsoleWindow.activeSelf; }
        set { ConsoleWindow.SetActive(value); }
    }

    private void Start()
    {
        Application.logMessageReceived += ProcessUnityLog;
    }

    private static void InitializeLogs()
    {
        logs = new Dictionary<LogTypes, string>()
        {
            { LogTypes.Everything, logEverything },
            { LogTypes.Errors, logErrors }
        };
        currentLogTypeToShow = LogTypes.Everything;
    }

    private void Awake()
    {
        ConsoleWindow = transform.Find("Console").gameObject;
        ConsoleOutput = transform.Find("Console").Find("Viewport").Find("Output").GetComponent<Text>();
    }

    private void Update ()
    {
        ProcessKeys();
    }

    private void ProcessKeys()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.BackQuote))
        {
            IsActive = !IsActive;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IsActive) ShowNextLog();
        }
    }

    public static void Write(string text, LogTypes logType = LogTypes.Everything, bool isBold = false, string color = "")
    {
        if (logs == null) InitializeLogs();

        string logString = text;
        if (isBold) logString = "<b>" + logString + "</b>";
        if (color != "") logString = "<color="+ color + ">" + logString + "</color>";

        logs[logType] += logString + "\n";

        if (logType != LogTypes.Everything)
        {
            logs[LogTypes.Everything] += logString + "\n";
        }

        if (currentLogTypeToShow == logType || currentLogTypeToShow == LogTypes.Everything)
        {
            ConsoleOutput.text = logs[currentLogTypeToShow];
            UpdateViewPosition();
        }
    }

    private static void UpdateViewPosition()
    {
        ConsoleWindow.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    private void ProcessUnityLog(string logString, string stackTrace, LogType type)
    {
        IsActive = true;
        Write("\n" + logString + "\n\n" + stackTrace, LogTypes.Errors, true, "red");
    }

    private void ShowNextLog()
    {
        if (currentLogTypeToShow == LogTypes.Everything) currentLogTypeToShow = LogTypes.Errors;
        else currentLogTypeToShow = LogTypes.Everything;

        ConsoleOutput.text = logs[currentLogTypeToShow];
        IsActive = false;
        IsActive = true;
        UpdateViewPosition();
    }
}

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

public class Console : MonoBehaviour {

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

    private static GameObject ConsoleWindow;
    private static GameObject ConsoleOutput;

    public static bool IsActive
    {
        get { return ConsoleWindow.activeSelf; }
        set { ConsoleWindow.SetActive(value); }
    }

    private static readonly int LOG_ENTRY_MARGIN = 10;
    private static float totalLogEntryHeight;

    private void Start()
    {
        Application.logMessageReceived += ProcessUnityLog;
    }

    private static void InitializeLogs()
    {
        logs = new List<LogEntry>();
        currentLogTypeToShow = LogTypes.Everything;
    }

    private void Awake()
    {
        ConsoleWindow = transform.Find("Console").gameObject;
        ConsoleOutput = transform.Find("Console").Find("Viewport").Find("Output").gameObject;
    }

    private void Update ()
    {
        ProcessKeys();
    }

    private void ProcessKeys()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (logs == null) InitializeLogs();
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

        LogEntry logEntry = new LogEntry(logString + "\n", logType);
        logs.Add(logEntry);

        if (currentLogTypeToShow == logType || currentLogTypeToShow == LogTypes.Everything)
        {
            ShowLogEntry(logEntry);
        }
    }

    private static void ShowLogEntry(LogEntry logEntry)
    {
        GameObject logEntryPrefab = (GameObject)Resources.Load("Prefabs/UI/LogEntry", typeof(GameObject));
        GameObject newLogEntry = Instantiate(logEntryPrefab, ConsoleOutput.transform);

        newLogEntry.GetComponent<Text>().text = logEntry.Text;

        float prefferedHeight = 0;

        if (logEntry.CalculatedPrefferedHeight == 0)
        {
            newLogEntry.GetComponent<RectTransform>().sizeDelta = new Vector2(
                Screen.width - 2 * LOG_ENTRY_MARGIN,
                0
            );

            prefferedHeight = newLogEntry.GetComponent<Text>().preferredHeight - 16;
            logEntry.CalculatedPrefferedHeight = prefferedHeight;
        }
        else
        {
            prefferedHeight = logEntry.CalculatedPrefferedHeight;
        }

        newLogEntry.GetComponent<RectTransform>().sizeDelta = new Vector2(
            Screen.width - 2 * LOG_ENTRY_MARGIN,
            prefferedHeight
        );

        newLogEntry.transform.localPosition = new Vector3(newLogEntry.transform.localPosition.x, -(LOG_ENTRY_MARGIN + totalLogEntryHeight));
        totalLogEntryHeight += newLogEntry.GetComponent<RectTransform>().sizeDelta.y;

        ConsoleOutput.GetComponent<RectTransform>().sizeDelta = new Vector2(ConsoleOutput.GetComponent<RectTransform>().sizeDelta.x, LOG_ENTRY_MARGIN + totalLogEntryHeight);

        UpdateViewPosition();
    }

    private static void UpdateViewPosition()
    {
        ConsoleWindow.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    private void ProcessUnityLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            IsActive = true;
            Write("\n" + logString + "\n\n" + stackTrace, LogTypes.Errors, true, "red");
        }
    }

    private void ShowNextLog()
    {
        if (currentLogTypeToShow == LogTypes.Everything) currentLogTypeToShow = LogTypes.Errors;
        else if (currentLogTypeToShow == LogTypes.Errors) currentLogTypeToShow = LogTypes.Triggers;
        else if (currentLogTypeToShow == LogTypes.Triggers) currentLogTypeToShow = LogTypes.Everything;

        ShowFilteredByType(currentLogTypeToShow);
    }

    private void ShowFilteredByType(LogTypes type)
    {
        foreach (Transform oldRecord in ConsoleOutput.transform)
        {
            Destroy(oldRecord.gameObject);
            totalLogEntryHeight = 0;
        }

        List<LogEntry> logEntriesToShow = (type == LogTypes.Everything) ? logs : logs.Where(n => n.Type == type).ToList();
        foreach (var filteredRecord in logEntriesToShow)
        {
            ShowLogEntry(filteredRecord);
        }
    }
}

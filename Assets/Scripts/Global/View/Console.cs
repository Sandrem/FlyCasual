using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public partial class Console : MonoBehaviour {

    private static GameObject ConsoleWindow;
    private static GameObject ConsoleOutput;

    public static bool IsActive
    {
        get { return ConsoleWindow.activeSelf; }
        set { ConsoleWindow.SetActive(value); }
    }

    private static readonly int LOG_ENTRY_MARGIN = 10;
    private static float totalLogEntryHeight;

    private void Awake()
    {
        ConsoleWindow = transform.Find("ConsoleWindow").gameObject;
        ConsoleOutput = transform.Find("ConsoleWindow").Find("ScrollRect").Find("Viewport").Find("Output").gameObject;
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
            if (IsActive)
            {
                ConsoleWindow.GetComponentInChildren<InputField>().Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IsActive) ShowNextLog();
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
                Screen.width - LOG_ENTRY_MARGIN,
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
        ConsoleWindow.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;
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

    public void OnEndEdit(GameObject input)
    {
        InputField inputField = input.GetComponent<InputField>();
        ProcessCommand(inputField.text);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }
}

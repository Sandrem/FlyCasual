﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ErrorReporter : MonoBehaviour
{
    public static ErrorReporter Instance;
    public GameObject ErrorReportPanel;
    public Text ErrorText;
    public Button OpenReplayLocationButton;
    public Button ReportButton;
    public Button CloseButton;
    

    public void Awake()
    {
        Instance = this;
        
        OpenReplayLocationButton.onClick.AddListener(OpenReplayLocationButtonEffect);
        ReportButton.onClick.AddListener(ReportButtonEffect);
        CloseButton.onClick.AddListener(CloseButtonEffect);
    }

    private void OpenReplayLocationButtonEffect()
    {
        try
        {
            Process.Start(Application.persistentDataPath + "/Second Edition/Replays");
        }
        catch (Exception)
        {
            Messages.ShowError("Cannot open file location");
        }
    }

    private void ReportButtonEffect()
    {
        Application.OpenURL("https://github.com/Baledin/FlyCasual/issues");
    }

    private void CloseButtonEffect()
    {
        Instance.ErrorReportPanel.SetActive(false);
    }

    public static void ShowError(string errorText)
    {
        Instance.ErrorReportPanel.SetActive(true);
        Instance.ErrorText.text = errorText;
    }
}

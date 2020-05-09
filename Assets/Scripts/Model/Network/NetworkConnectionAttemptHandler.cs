using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionAttemptHandler : MonoBehaviour
{
    public InputField IpInput;
    public GameObject BottomPanel;
    private bool IsTracking;

    public void StartAttempt()
    {
        BottomPanel.SetActive(false);
        IpInput.interactable = false;
        IsTracking = true;
    }

    public void StopAtempt()
    {
        IsTracking = false;
    }

    void Update()
    {
        if (IsTracking)
        {
            if (!NetworkManager.singleton.isNetworkActive)
            {
                AbortAttempt();
            }
        }
    }

    private void AbortAttempt()
    {
        NetworkManager.singleton.StopClient();
        Messages.ShowError("Connection failed");
        IsTracking = false;
        BottomPanel.SetActive(true);
        IpInput.interactable = true;
    }
}

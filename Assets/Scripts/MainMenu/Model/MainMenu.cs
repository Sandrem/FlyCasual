using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainMenu : MonoBehaviour {

    // Use this for initialization
    void Start () {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        SetPositions();
        SetCurrentPanel();
    }

    public void StartBattle()
    {
        RosterBuilder.StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

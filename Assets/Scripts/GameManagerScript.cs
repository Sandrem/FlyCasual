using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void CallBackFunction();

public class GameManagerScript : MonoBehaviour {

    public PrefabsList PrefabsList;

    public UI UI;
    public ShipMovementScript Movement;
    public ShipPositionManager Position;

    void Start() {
        SetApplicationParameters();
        InitializeScripts();

        Global.Initialize();

        Board.BoardManager.Initialize();
        Roster.Initialize();
        Roster.Start();
        Phases.StartPhases();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.ToggleInGameMenu();
        }
    }

    private void SetApplicationParameters()
    {
        Application.targetFrameRate = 100;

        Options.UpdateVolume();
    }

    private void InitializeScripts()
    {
        PrefabsList = this.GetComponent<PrefabsList>();
        UI = this.GetComponent<UI>();
        
        Movement = this.GetComponent<ShipMovementScript>();
        Position = this.GetComponent<ShipPositionManager>();
    }

    public void Wait(float seconds, CallBackFunction callBack)
    {
        StartCoroutine(WaitCoroutine(seconds, callBack));
    }

    IEnumerator WaitCoroutine(float seconds, CallBackFunction callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }

}

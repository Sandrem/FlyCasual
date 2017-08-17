using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CallBackFunction();

public class GameManagerScript : MonoBehaviour {

    //Move to board consts
    public readonly float PLAYMAT_SIZE = 10;

    public PrefabsList PrefabsList;

    public UI UI;
    public ShipMovementScript Movement;
    public ShipPositionManager Position;

    void Start() {
        SetApplicationParameters();
        InitializeScripts();

        Global.Initialize();

        Roster.Start();
        Phases.StartPhases();
    }

    private void SetApplicationParameters()
    {
        Application.targetFrameRate = 100;
    }

    private void InitializeScripts()
    {
        PrefabsList = this.GetComponent<PrefabsList>();
        UI = this.GetComponent<UI>();
        UI.Initialize();
        
        Movement = this.GetComponent<ShipMovementScript>();
        Movement.Initialize();
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

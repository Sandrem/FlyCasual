using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Application.targetFrameRate = 60;
    }

    private void InitializeScripts()
    {
        PrefabsList = this.GetComponent<PrefabsList>();

        UI = this.GetComponent<UI>();
        
        Movement = this.GetComponent<ShipMovementScript>();
        
        Position = this.GetComponent<ShipPositionManager>();
    }

}

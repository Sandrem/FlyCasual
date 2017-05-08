using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    //Move to board consts
    public readonly float PLAYMAT_SIZE = 10;

    public PhaseManagerScript PhaseManager;
    public UIManagerScript UI;
    public ShipActionsManagerScript Actions;
    public CombatManagerScript Combat;
    public ShipRosterManagerScript Roster;
    public ShipSelectionManagerScript Selection;
    public ShipMovementScript Movement;
    public DiceManagementScript Dices;
    public ShipFactoryScript ShipFactory;
    public RulerManagement Ruler;
    public ShipPositionManager Position;
    public Rules.RulesScript Rules { get; private set; }

    void Start() {
        InitializeScripts();
        SetApplicationParemeters();
        Roster.SpawnAllShips();
        PhaseManager.StartPhases();
        //Debug.Log(Global.test);
    }

    void Update()
    {

    }

    private void InitializeScripts()
    {
        PhaseManager = this.GetComponent<PhaseManagerScript>();
        UI = this.GetComponent<UIManagerScript>();
            UI.ErrorManager = this.GetComponent<MessageManagerScript>();
            UI.Roster = this.GetComponent<RosterInfoScript>();
            UI.DiceResults = this.GetComponent<DiceResultsScript>();
            UI.ActionsPanel = this.GetComponent<ActionsPanelScript>();
            UI.Helper = this.GetComponent<HelpInfoScript>();
        Actions = this.GetComponent<ShipActionsManagerScript>();
        Combat = this.GetComponent<CombatManagerScript>();
        Roster = this.GetComponent<ShipRosterManagerScript>();
        Selection = this.GetComponent<ShipSelectionManagerScript>();
        Movement = this.GetComponent<ShipMovementScript>();
            Movement.Ruler = this.GetComponent<RulerManagement>();
        Dices = this.GetComponent<DiceManagementScript>();
        Ruler = this.GetComponent<RulerManagement>();
        ShipFactory = this.GetComponent<ShipFactoryScript>();
        Position = this.GetComponent<ShipPositionManager>();

        Rules = new Rules.RulesScript(this);
    }

    private void SetApplicationParemeters()
    {
        Application.targetFrameRate = 60;
    }

}

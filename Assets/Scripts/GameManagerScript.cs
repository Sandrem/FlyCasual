using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    //Move to board consts
    public readonly float PLAYMAT_SIZE = 10;

    public ShipRoster Roster;

    public ShipFactory ShipFactory;
    public PrefabsList PrefabList;

    public PhaseManagerScript PhaseManager;
    public UIManagerScript UI;
    public ShipActionsManagerScript Actions;
    public CombatManagerScript Combat;
    
    public ShipSelectionManagerScript Selection;
    public ShipMovementScript Movement;
    public DiceManagementScript Dices;
    public RulerManagement Ruler;
    public CriticalHitsDeck CritsDeck;
    public ShipPositionManager Position;
    public Rules.RulesScript Rules { get; private set; }

    void Start() {
        InitializeScripts();
        SetApplicationParemeters();
        Roster.Start();
        CritsDeck.InitializeDeck();
        PhaseManager.StartPhases();
    }

    void Update()
    {

    }

    private void InitializeScripts()
    {
        Roster = new ShipRoster();

        PhaseManager = this.GetComponent<PhaseManagerScript>();
        UI = this.GetComponent<UIManagerScript>();
            UI.ErrorManager = this.GetComponent<MessageManagerScript>();
            UI.DiceResults = this.GetComponent<DiceResultsScript>();
            UI.ActionsPanel = this.GetComponent<ActionsPanelScript>();
            UI.Helper = this.GetComponent<HelpInfoScript>();
        Actions = this.GetComponent<ShipActionsManagerScript>();
        Combat = this.GetComponent<CombatManagerScript>();
        Selection = this.GetComponent<ShipSelectionManagerScript>();
        Movement = this.GetComponent<ShipMovementScript>();
            Movement.Ruler = this.GetComponent<RulerManagement>();
        Dices = this.GetComponent<DiceManagementScript>();
        Ruler = this.GetComponent<RulerManagement>();
        Position = this.GetComponent<ShipPositionManager>();
        CritsDeck = this.GetComponent<CriticalHitsDeck>();

        PrefabList = this.GetComponent<PrefabsList>();

        Rules = new Rules.RulesScript(this);
    }

    private void SetApplicationParemeters()
    {
        Application.targetFrameRate = 60;
    }

}

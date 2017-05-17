using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    //Move to board consts
    public readonly float PLAYMAT_SIZE = 10;

    public ShipRoster Roster;

    private ShipFactory shipFactory;
    public ShipFactory ShipFactory
    {
        get 
        {
            if (this.shipFactory == null) { shipFactory = new ShipFactory(); }
            return shipFactory;
        }
    }

    public PrefabsList PrefabList;
    public PhasesManager Phases;
    public ShipActionsManager Actions;
    public DiceManager Dices;
    public MovementTemplates MovementTemplates;
    public CriticalHitsDeckManager CritsDeck;
    public Rules.RulesScript Rules;

    public UIManagerScript UI;
    public CombatManager Combat;
    public ShipSelectionManagerScript Selection;
    public ShipMovementScript Movement;
    public ShipPositionManager Position;


    void Start() {
        InitializeScripts();
        SetApplicationParemeters();

        //Start Board
        MovementTemplates.Start();
        Dices.Start();

        //Rules Start
        Actions.Start();
        Combat.Start();

        Roster.Start();
        CritsDeck.InitializeDeck();
        Phases.StartPhases();
    }

    void Update()
    {

    }

    private void InitializeScripts()
    {
        Roster = new ShipRoster();
        Phases = new PhasesManager();
        MovementTemplates = new MovementTemplates();
        Dices = new DiceManager();
        CritsDeck = new CriticalHitsDeckManager();
        Actions = new ShipActionsManager();
        Combat = new CombatManager();
        Rules = new Rules.RulesScript(this);

        PrefabList = this.GetComponent<PrefabsList>();

        UI = this.GetComponent<UIManagerScript>();
            UI.ErrorManager = this.GetComponent<MessageManagerScript>();
            UI.DiceResults = this.GetComponent<DiceResultsScript>();
            UI.ActionsPanel = this.GetComponent<ActionsPanelScript>();
        
        
        Selection = this.GetComponent<ShipSelectionManagerScript>();
        Movement = this.GetComponent<ShipMovementScript>();
        
        Position = this.GetComponent<ShipPositionManager>();
    }

    private void SetApplicationParemeters()
    {
        Application.targetFrameRate = 60;
    }

}

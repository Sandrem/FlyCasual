using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
// Fix PilotSkillSubPhasePlayer

/*
* DESCRIPTION
* Controls phases of game
* Call function NextPhase and NextSubPhase to go throught game's steps
* Displays help information using HelpInfoScript UI
*/

public class PhaseManagerScript: MonoBehaviour {

    private GameManagerScript Game;

    public GenericPhase CurrentPhase { get; set; }

    public Player PlayerWithInitiative;

    //EVENTS
    public delegate void EventHandler();
    public event EventHandler OnSetupPhaseStart;
    public event EventHandler OnPlanningPhaseStart;
    public event EventHandler OnActivationPhaseStart;
    public event EventHandler OnCombatPhaseStart;
    public event EventHandler OnEndPhaseStart;


    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    //Public funtions

    public void StartPhases()
    {
        //fix this
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        CurrentPhase = new SetupPhase();
        Game.UI.AddTestLogEntry("Game is started");
        CurrentPhase.StartPhase();
    }

    public void CallNextSubPhase()
    {
        Game.UI.HideTemporaryMenus();
        Game.Combat.ReturnRangeRuler();

        if (CurrentPhase.Phase == Phases.Activation)
        {
            if (!Game.Roster.AllManueversArePerformed())
            {
                return;
            }
        }

        CurrentPhase.NextSubPhase();
    }

    public void CallSetupPhaseTrigger()
    {
        if (OnSetupPhaseStart != null) OnSetupPhaseStart();
    }

    public void CallPlanningPhaseTrigger()
    {
        if (OnPlanningPhaseStart != null) OnPlanningPhaseStart();
    }

    public void CallActivationPhaseTrigger()
    {
        if (OnActivationPhaseStart != null) OnActivationPhaseStart();
    }

    public void CallCombatPhaseTrigger()
    {
        if (OnCombatPhaseStart != null) OnCombatPhaseStart();
    }

    public void CallEndPhaseTrigger()
    {
        if (OnEndPhaseStart != null) OnEndPhaseStart();
    }

}

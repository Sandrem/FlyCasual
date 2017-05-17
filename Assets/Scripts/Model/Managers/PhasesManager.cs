using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phases;
using SubPhases;
using Players;

//TODO
// Fix PilotSkillSubPhasePlayer

/*
* DESCRIPTION
* Controls phases of game
* Call function NextPhase and NextSubPhase to go throught game's steps
* Displays help information using HelpInfoScript UI
*/

public partial class PhasesManager {

    private GameManagerScript Game;

    public GenericPhase CurrentPhase { get; set; }
    public GenericSubPhase CurrentSubPhase { get; set; }

    private bool inTemporarySubPhase;
    public bool InTemporarySubPhase
    {
        get { return CurrentSubPhase.isTemporary; }
    }

    public PlayerNo PlayerWithInitiative = PlayerNo.Player1;

    private PlayerNo currentPhasePlayer;
    public PlayerNo CurrentPhasePlayer
    {
        get { return CurrentSubPhase.RequiredPlayer; }
    }

    //EVENTS
    public delegate void EventHandler();
    public event EventHandler OnSetupPhaseStart;
    public event EventHandler OnPlanningPhaseStart;
    public event EventHandler OnActivationPhaseStart;
    public event EventHandler OnCombatPhaseStart;
    public event EventHandler OnEndPhaseStart;

    //PHASES CONTROL

    public void StartPhases()
    {
        //Todo: Create starting point
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        CurrentPhase = new SetupPhase();
        Game.UI.AddTestLogEntry("Game is started");
        CurrentPhase.StartPhase();
    }

    public void Next()
    {
        CurrentSubPhase.NextSubPhase();
    }

    public void NextPhase()
    {
        CurrentPhase.NextPhase();
    }

    //TRIGGERS

    public void CallNextSubPhase()
    {
        CurrentSubPhase.CallNextSubPhase();
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

    //TEMPORARY SUBPHASES

    public void StartFreeActionSubPhase(string name)
    {
        StartTemporarySubPhase(name, new FreeActionSubPhase());
    }

    public void StartSelectTargetSubPhase(string name)
    {
        StartTemporarySubPhase(name, new SelectTargetSubPhase());
    }

    public void StartBarrelRollSubPhase(string name)
    {
        StartTemporarySubPhase(name, new BarrelRollSubPhase());
    }

    private void StartTemporarySubPhase(string name, GenericSubPhase subPhase)
    {
        GenericSubPhase previousSubPhase = CurrentSubPhase;
        CurrentSubPhase = subPhase;
        CurrentSubPhase.Name = name;
        CurrentSubPhase.PreviousSubPhase = previousSubPhase;
        CurrentSubPhase.RequiredPlayer = previousSubPhase.RequiredPlayer;
        CurrentSubPhase.RequiredPilotSkill = previousSubPhase.RequiredPilotSkill;
        CurrentSubPhase.StartSubPhase();
    }

}

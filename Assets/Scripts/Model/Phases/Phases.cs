using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainPhases;
using SubPhases;
using Players;
using System;

public static partial class Phases
{
    public static int RoundCounter;
    public static bool GameIsEnded;

    public static GenericPhase CurrentPhase { get; set; }
    public static GenericSubPhase CurrentSubPhase { get; set; }

    private static bool inTemporarySubPhase;
    public static bool InTemporarySubPhase
    {
        get { return CurrentSubPhase.IsTemporary; }
    }

    public static PlayerNo PlayerWithInitiative = PlayerNo.Player1;

    private static PlayerNo currentPhasePlayer;
    public static PlayerNo CurrentPhasePlayer
    {
        get { return CurrentSubPhase.RequiredPlayer; }
    }

    // EVENTS
    public delegate void EventHandler();
    public static event EventHandler OnGameStart;
    public static event EventHandler OnRoundStart;
    public static event EventHandler OnSetupPhaseStart;
    public static event EventHandler OnPlanningPhaseStart;
    public static event EventHandler OnActivationPhaseStart;
    public static event EventHandler BeforeActionSubPhaseStart;
    public static event EventHandler OnActionSubPhaseStart;
    public static event EventHandler OnActivationPhaseEnd;
    public static event EventHandler OnCombatPhaseStart;
    public static event EventHandler OnCombatPhaseEnd;
    public static event EventHandler OnCombatSubPhaseRequiredPilotSkillIsChanged;
    public static event EventHandler OnEndPhaseStart;
    public static event EventHandler OnRoundEnd;

    // PHASES CONTROL

    public static void StartPhases()
    {
        CallGameStart();
        StartGame();
    }

    private static void CallGameStart()
    {
        if (OnGameStart != null) OnGameStart();
    }

    private static void StartGame()
    {
        RoundCounter = 0;
        GameIsEnded = false;
        CurrentPhase = new SetupPhase();
        UI.AddTestLogEntry("Game is started");
        CurrentPhase.StartPhase();
    }

    public static void FinishSubPhase(System.Type subPhaseType)
    {
        if (CurrentSubPhase.GetType() == subPhaseType)
        {
            if (DebugManager.DebugPhases) Debug.Log("Phase " + subPhaseType + "is finished directly");
            Next();
        }
        else
        {
            Debug.Log("OOPS! YOU WANT TO FINISH " + subPhaseType + " SUBPHASE, BUT NOW IS " + CurrentSubPhase.GetType() + " SUBPHASE!");
        }
    }

    public static void Next()
    {
        //Debug.Log("NEXT - finish for: " + CurrentSubPhase);
        CurrentSubPhase.Next();
    }

    public static void NextPhase()
    {
        CurrentPhase.NextPhase();
    }

    public static void CallNextSubPhase()
    {
        CurrentSubPhase.CallNextSubPhase();
    }

    // TRIGGERS

    public static void CallRoundStartTrigger()
    {
        if (OnRoundStart != null) OnRoundStart();
    }

    public static void CallSetupPhaseTrigger()
    {
        if (OnSetupPhaseStart != null) OnSetupPhaseStart();

        Triggers.ResolveTriggers(TriggerTypes.OnSetupPhaseStart, delegate() { FinishSubPhase(typeof(SetupStartSubPhase)); });
    }

    public static void CallPlanningPhaseTrigger()
    {
        if (OnPlanningPhaseStart != null) OnPlanningPhaseStart();
    }

    public static void CallActivationPhaseStartTrigger()
    {
        if (OnActivationPhaseStart != null) OnActivationPhaseStart();
        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnActivationPhaseStart();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnActivationPhaseStart, delegate () { FinishSubPhase(typeof(ActivationStartSubPhase)); });
    }

    public static void CallActivationPhaseEndTrigger()
    {
        if (OnActivationPhaseEnd!= null) OnActivationPhaseEnd();

        Triggers.ResolveTriggers(TriggerTypes.OnActivationPhaseEnd, delegate () { FinishSubPhase(typeof(ActivationEndSubPhase)); });
    }


    public static void CallCombatPhaseStartTrigger()
    {
        if (OnCombatPhaseStart != null) OnCombatPhaseStart();
        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnCombatPhaseStart();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnCombatPhaseStart, delegate () { FinishSubPhase(typeof(CombatStartSubPhase)); });
    }

    public static void CallCombatPhaseEndTrigger()
    {
        if (OnCombatPhaseEnd != null) OnCombatPhaseEnd();
        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnCombatPhaseEnd();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnCombatPhaseEnd, delegate () { FinishSubPhase(typeof(CombatEndSubPhase)); });
    }

    public static void CallEndPhaseTrigger(Action callBack)
    {
        if (OnEndPhaseStart != null) OnEndPhaseStart();

        Triggers.ResolveTriggers(TriggerTypes.OnEndPhaseStart, callBack);
    }

    public static void CallRoundEndTrigger()
    {
        if (OnRoundEnd != null) OnRoundEnd();
    }

    public static void CallBeforeActionSubPhaseTrigger()
    {
        if (BeforeActionSubPhaseStart != null) BeforeActionSubPhaseStart();
    }

    public static void CallCombatSubPhaseRequiredPilotSkillIsChanged()
    {
        if (OnCombatSubPhaseRequiredPilotSkillIsChanged != null) OnCombatSubPhaseRequiredPilotSkillIsChanged();
    }

    public static void CallOnActionSubPhaseTrigger()
    {
        if (OnActionSubPhaseStart != null) OnActionSubPhaseStart();
        Selection.ThisShip.CallOnActionSubPhaseStart();

        Triggers.ResolveTriggers(TriggerTypes.OnActionSubPhaseStart, delegate () { FinishSubPhase(typeof(ActionSubPhase)); });
    }

    // TEMPORARY SUBPHASES

    public static void StartTemporarySubPhase(string name, System.Type subPhaseType, Action callBack = null)
    {
        CurrentSubPhase.Pause();
        if (DebugManager.DebugPhases) Debug.Log("Temporary phase " + subPhaseType + " is started directly");
        GenericSubPhase previousSubPhase = CurrentSubPhase;
        CurrentSubPhase = (GenericSubPhase)System.Activator.CreateInstance(subPhaseType);
        CurrentSubPhase.Name = name;
        CurrentSubPhase.CallBack = callBack;
        CurrentSubPhase.PreviousSubPhase = previousSubPhase;
        CurrentSubPhase.RequiredPlayer = previousSubPhase.RequiredPlayer;
        CurrentSubPhase.RequiredPilotSkill = previousSubPhase.RequiredPilotSkill;
        CurrentSubPhase.Start();
    }

 }



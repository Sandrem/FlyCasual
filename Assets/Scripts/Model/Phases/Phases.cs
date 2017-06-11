using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainPhases;
using SubPhases;
using Players;

public static partial class Phases
{

    private static GameManagerScript Game;

    public static GenericPhase CurrentPhase { get; set; }
    public static GenericSubPhase CurrentSubPhase { get; set; }

    private static bool inTemporarySubPhase;
    public static bool InTemporarySubPhase
    {
        get { return CurrentSubPhase.isTemporary; }
    }

    public static PlayerNo PlayerWithInitiative = PlayerNo.Player1;

    private static PlayerNo currentPhasePlayer;
    public static PlayerNo CurrentPhasePlayer
    {
        get { return CurrentSubPhase.RequiredPlayer; }
    }

    private static List<System.Type> subPhasesToStart = new List<System.Type>();
    private static List<System.Type> subPhasesToFinish = new List<System.Type>();

    //EVENTS
    public delegate void EventHandler();
    public static event EventHandler OnRoundStart;
    public static event EventHandler OnSetupPhaseStart;
    public static event EventHandler OnPlanningPhaseStart;
    public static event EventHandler OnActivationPhaseStart;
    public static event EventHandler OnCombatPhaseStart;
    public static event EventHandler OnEndPhaseStart;

    public static event EventHandler OnActionSubPhaseStart;

    //PHASES CONTROL

    public static void StartPhases()
    {
        //Todo: Create starting point
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        CurrentPhase = new SetupPhase();
        Game.UI.AddTestLogEntry("Game is started");
        CurrentPhase.StartPhase();
    }

    public static void FinishSubPhase(System.Type subPhaseType)
    {
        if (CurrentSubPhase.GetType() == subPhaseType)
        {
            Next();
        }
        else
        {
            if (!subPhasesToFinish.Contains(subPhaseType))
            {
                subPhasesToFinish.Add(subPhaseType);
            }
        }
    }

    public static void Next()
    {
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

    public static void CheckScheduledChanges()
    {
        if (subPhasesToFinish.Count == 0) return;

        List<System.Type> tempList = new List<System.Type>();
        foreach (var subPhaseType in subPhasesToFinish)
        {
            tempList.Add(subPhaseType);
        }

        foreach (var subPhaseType in tempList)
        {
            if (CurrentSubPhase.GetType() == subPhaseType)
            {
                subPhasesToFinish.Remove(subPhaseType);
                Next();
            }
        }
    }

    //TRIGGERS

    public static void CallRoundStartTrigger()
    {
        if (OnSetupPhaseStart != null) OnRoundStart();
    }

    public static void CallSetupPhaseTrigger()
    {
        if (OnSetupPhaseStart != null) OnSetupPhaseStart();
    }

    public static void CallPlanningPhaseTrigger()
    {
        if (OnPlanningPhaseStart != null) OnPlanningPhaseStart();
    }

    public static void CallActivationPhaseTrigger()
    {
        if (OnActivationPhaseStart != null) OnActivationPhaseStart();
    }

    public static void CallCombatPhaseTrigger()
    {
        if (OnCombatPhaseStart != null) OnCombatPhaseStart();
        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnCombatPhaseStart();
        }
    }

    public static void CallEndPhaseTrigger()
    {
        if (OnEndPhaseStart != null) OnEndPhaseStart();
    }

    public static void CallOnActionSubPhaseTrigger()
    {
        if (OnActionSubPhaseStart != null) OnActionSubPhaseStart();
    }

    //TEMPORARY SUBPHASES

    public static void StartFreeActionSubPhase(string name)
    {
        StartTemporarySubPhase(name, new FreeActionSubPhase());
    }

    public static void StartSelectTargetSubPhase(string name)
    {
        StartTemporarySubPhase(name, new SelectTargetSubPhase());
    }

    public static void StartBarrelRollSubPhase(string name)
    {
        StartTemporarySubPhase(name, new BarrelRollSubPhase());
    }

    public static void StartKoiogranTurnSubPhase(string name)
    {
        StartTemporarySubPhase(name, new KoiogranTurnSubPhase());
    }

    public static void StartDiceRollSubPhase(string name)
    {
        StartTemporarySubPhase(name, new DiceRollSubPhase());
    }

    public static void StartMovementExecutionSubPhase(string name)
    {
        StartTemporarySubPhase(name, new MovementExecutionSubPhase());
    }

    public static void StartRepositionExecutionSubPhase(string name)
    {
        StartTemporarySubPhase(name, new RepositionExecutionSubPhase());
    }

    private static void StartTemporarySubPhase(string name, GenericSubPhase subPhase)
    {
        GenericSubPhase previousSubPhase = CurrentSubPhase;
        CurrentSubPhase = subPhase;
        CurrentSubPhase.Name = name;
        CurrentSubPhase.PreviousSubPhase = previousSubPhase;
        CurrentSubPhase.RequiredPlayer = previousSubPhase.RequiredPlayer;
        CurrentSubPhase.RequiredPilotSkill = previousSubPhase.RequiredPilotSkill;
        CurrentSubPhase.Start();
    }

}

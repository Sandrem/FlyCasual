using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainPhases;
using SubPhases;
using Players;
using System;

public static partial class Phases
{

    private static GameManagerScript Game;

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

    private static List<System.Type> subPhasesToStart = new List<System.Type>();
    private static List<System.Type> subPhasesToFinish = new List<System.Type>();

    // EVENTS
    public delegate void EventHandler();
    public static event EventHandler OnRoundStart;
    public static event EventHandler OnSetupPhaseStart;
    public static event EventHandler OnPlanningPhaseStart;
    public static event EventHandler OnActivationPhaseStart;
    public static event EventHandler BeforeActionSubPhaseStart;
    public static event EventHandler OnActionSubPhaseStart;
    public static event EventHandler OnCombatPhaseStart;
    public static event EventHandler OnEndPhaseStart;

    // PHASES CONTROL

    public static void StartPhases()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        CurrentPhase = new SetupPhase();
        Game.UI.AddTestLogEntry("Game is started");
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
            if (DebugManager.DebugPhases) Debug.Log("OOPS! YOU WANT TO FINISH " + subPhaseType + " SUBPHASE, BUT NOW IS " + CurrentSubPhase.GetType() + " SUBPHASE!");
            /*if (!subPhasesToFinish.Contains(subPhaseType))
            {
                Debug.Log("Phase " + subPhaseType + " is planned to finish");
                subPhasesToFinish.Add(subPhaseType);
            }*/
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

    public static void CheckScheduledChanges()
    {
        CheckScheduledFinishes();
        CheckScheduledStarts();
    }

    private static void CheckScheduledFinishes()
    {
        if (subPhasesToFinish.Count != 0)
        {
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
    }

    public static void CancelScheduledFinish(System.Type subPhaseType)
    {
        if (subPhasesToFinish.Contains(subPhaseType))
        {
            Debug.Log(subPhaseType + " is removed from scheduled finish list");
            subPhasesToFinish.Remove(subPhaseType);
        }
    }

    private static void CheckScheduledStarts()
    {
        if (!InTemporarySubPhase)
        {
            if (subPhasesToStart.Count != 0)
            {
                StartTemporarySubPhase("SCHEDULED", subPhasesToStart[0]);
                subPhasesToStart.RemoveAt(0);
            }
        }
    }

    public static IEnumerator WaitForTemporarySubPhasesFinish()
    {
        while (Phases.CurrentSubPhase.IsTemporary)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // TRIGGERS

    public static void CallRoundStartTrigger()
    {
        if (OnRoundStart != null) OnRoundStart();
    }

    public static IEnumerator CallSetupPhaseTrigger()
    {
        if (OnSetupPhaseStart != null) OnSetupPhaseStart();

        yield return Triggers.ResolveAllTriggers(TriggerTypes.OnSetupPhaseStart);
        yield return Phases.WaitForTemporarySubPhasesFinish();
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
        Game.StartCoroutine(ResolveCombatTriggers());
    }

    private static IEnumerator ResolveCombatTriggers()
    {
        yield return Triggers.ResolveAllTriggers(TriggerTypes.OnCombatPhaseStart);
        Phases.FinishSubPhase(typeof(CombatStartSubPhase));
    }

    public static void CallEndPhaseTrigger()
    {
        if (OnEndPhaseStart != null) OnEndPhaseStart();
    }

    public static void CallBeforeActionSubPhaseTrigger()
    {
        if (BeforeActionSubPhaseStart != null) BeforeActionSubPhaseStart();
    }

    public static void CallOnActionSubPhaseTrigger()
    {
        if (OnActionSubPhaseStart != null) OnActionSubPhaseStart();
        Selection.ThisShip.CallOnActionSubPhaseStart();

        Game.StartCoroutine(ResolveActionTriggers());
    }

    private static IEnumerator ResolveActionTriggers()
    {
        yield return Triggers.ResolveAllTriggers(TriggerTypes.OnActionSubPhaseStart);
        yield return Phases.WaitForTemporarySubPhasesFinish();
        FinishSubPhase(typeof(ActionSubPhase));
    }

    // TEMPORARY SUBPHASES

    public static void StartTemporarySubPhase(string name, System.Type subPhaseType)
    {
        CurrentSubPhase.Pause();
        if (DebugManager.DebugPhases) Debug.Log("Temporary phase " + subPhaseType + " is started directly");
        GenericSubPhase previousSubPhase = CurrentSubPhase;
        CurrentSubPhase = (GenericSubPhase)System.Activator.CreateInstance(subPhaseType);
        CurrentSubPhase.Name = name;
        CurrentSubPhase.PreviousSubPhase = previousSubPhase;
        CurrentSubPhase.RequiredPlayer = previousSubPhase.RequiredPlayer;
        CurrentSubPhase.RequiredPilotSkill = previousSubPhase.RequiredPilotSkill;
        CurrentSubPhase.Start();
    }

 }



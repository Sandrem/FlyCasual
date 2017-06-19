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

    // EVENTS
    public delegate void EventHandler();
    public static event EventHandler OnRoundStart;
    public static event EventHandler OnSetupPhaseStart;
    public static event EventHandler OnPlanningPhaseStart;
    public static event EventHandler OnActivationPhaseStart;
    public static event EventHandler OnCombatPhaseStart;
    public static event EventHandler OnEndPhaseStart;

    public static event EventHandler OnActionSubPhaseStart;

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
            Debug.Log("Phase " + subPhaseType + "is finished directly");
            Next();
        }
        else
        {
            Debug.Log("Oops! You want to finish " + subPhaseType +" subphase, but now is " + CurrentSubPhase.GetType() + " subphase!");
            if (!subPhasesToFinish.Contains(subPhaseType))
            {
                Debug.Log("Phase " + subPhaseType + " is planned to finish");
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
        while (Phases.CurrentSubPhase.isTemporary)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // TRIGGERS

    public static void CallRoundStartTrigger()
    {
        if (OnRoundStart != null) OnRoundStart();
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
        Game.StartCoroutine(ResolveCombatTriggers());
    }

    private static IEnumerator ResolveCombatTriggers()
    {
        yield return Triggers.ResolveAllTriggers(TriggerTypes.OnCombatPhaseStart);
        Debug.Log("All pre-Combat Triggers are resolved, START OF COMBAT!");
        CurrentSubPhase.Initialize();
    }

    public static void CallEndPhaseTrigger()
    {
        if (OnEndPhaseStart != null) OnEndPhaseStart();
    }

    public static void CallOnActionSubPhaseTrigger()
    {
        if (OnActionSubPhaseStart != null) OnActionSubPhaseStart();
    }

    // TEMPORARY SUBPHASES

    public static void StartTemporarySubPhase(string name, System.Type subPhaseType)
    {
        if (!InTemporarySubPhase)
        {
            Debug.Log("Temporary phase " + subPhaseType + " is started directly");
            GenericSubPhase previousSubPhase = CurrentSubPhase;
            CurrentSubPhase = (GenericSubPhase)System.Activator.CreateInstance(subPhaseType);
            CurrentSubPhase.Name = name;
            CurrentSubPhase.PreviousSubPhase = previousSubPhase;
            CurrentSubPhase.RequiredPlayer = previousSubPhase.RequiredPlayer;
            CurrentSubPhase.RequiredPilotSkill = previousSubPhase.RequiredPilotSkill;
            CurrentSubPhase.Start();
        }
        else
        {
            Debug.Log("Temporary phase " + subPhaseType + " start is delayed");
            subPhasesToStart.Add(subPhaseType);
        }
    }

 }



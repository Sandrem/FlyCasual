using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainPhases;
using SubPhases;
using Players;
using System;
using BoardTools;
using GameModes;

public static partial class Phases
{
    public static int RoundCounter;
    public static bool GameIsEnded;

    public static GenericPhase CurrentPhase { get; set; }
    public static GenericSubPhase CurrentSubPhase { get; set; }

    public static PhaseEvents Events;

    public static bool InTemporarySubPhase
    {
        get { return CurrentSubPhase.IsTemporary; }
    }

    public static PlayerNo PlayerWithInitiative = PlayerNo.Player1;

    public static PlayerNo CurrentPhasePlayer
    {
        get { return CurrentSubPhase.RequiredPlayer; }
    }

    // PHASES CONTROL

    public static void Initialize()
    {
        Events = new PhaseEvents();
    }

    public static void StartPhases()
    {
        StartGame();
    }

    private static void StartGame()
    {
        RoundCounter = 0;
        GameIsEnded = false;
        CurrentPhase = new SetupPhase();

        Events.CallGameStartTrigger(CurrentPhase.StartPhase);
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

    // TEMPORARY SUBPHASES

    public static void StartTemporarySubPhaseOld(string name, System.Type subPhaseType, Action callBack = null)
    {
        GenericSubPhase subphase = StartTemporarySubPhaseNew(name, subPhaseType, callBack);
        subphase.Start();
    }

    public static GenericSubPhase StartTemporarySubPhaseNew(string name, System.Type subPhaseType, Action callBack)
    {
        if (CurrentSubPhase != null) CurrentSubPhase.Pause();

        if (DebugManager.DebugPhases) Debug.Log("Temporary phase " + subPhaseType + " is started directly");
        GenericSubPhase previousSubPhase = CurrentSubPhase;
        CurrentSubPhase = (GenericSubPhase)System.Activator.CreateInstance(subPhaseType);
        CurrentSubPhase.Name = name;
        CurrentSubPhase.CallBack = callBack;
        CurrentSubPhase.PreviousSubPhase = previousSubPhase;

        if (previousSubPhase != null)
        {
            CurrentSubPhase.RequiredPlayer = previousSubPhase.RequiredPlayer;
            CurrentSubPhase.RequiredPilotSkill = previousSubPhase.RequiredPilotSkill;
        }

        return CurrentSubPhase;
    }

    public static T StartTemporarySubPhaseNew<T>(string name, Action callBack) where T : GenericSubPhase, new()
    {
        return (T)StartTemporarySubPhaseNew(name, typeof(T), callBack);
    }

    public static void EndGame()
    {
        Events.CallEndGame();

        GameIsEnded = true;

        foreach (var shipHolder in Roster.AllShips)
        {
            foreach (var ability in shipHolder.Value.PilotAbilities)
            {
                ability.DeactivateAbility();
            }

            foreach (var upgrade in shipHolder.Value.UpgradeBar.GetUpgradesOnlyFaceup())
            {
                foreach (var upgradeAbility in upgrade.UpgradeAbilities)
                {
                    upgradeAbility.DeactivateAbility();
                }
            }
        }

        Board.Cleanup();
    }

    public static void GoBack(Type specificSubphaseToFinish = null)
    {
        DecisionSubPhase decisionSubphase = CurrentSubPhase.PreviousSubPhase as DecisionSubPhase;
        if (decisionSubphase != null) decisionSubphase.DecisionWasPreparedAndShown = false;

        Type subphaseToFinish = specificSubphaseToFinish ?? CurrentSubPhase.GetType();
        FinishSubPhase(subphaseToFinish);
        CurrentSubPhase.Resume();
    }

    public static void Skip()
    {
        GameMode.CurrentGameMode.SkipButtonEffect();
    }

}



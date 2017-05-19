using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainPhases;
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

public static partial class Phases
{

    public static void FinishSubPhase(System.Type subPhaseType)
    {
        if (CurrentSubPhase.GetType() == subPhaseType) Next();
    }

    public static void Next()
    {
        CurrentSubPhase.NextSubPhase();
    }

    public static void NextPhase()
    {
        CurrentPhase.NextPhase();
    }

    //TRIGGERS

    public static void CallNextSubPhase()
    {
        CurrentSubPhase.CallNextSubPhase();
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
    }

    public static void CallEndPhaseTrigger()
    {
        if (OnEndPhaseStart != null) OnEndPhaseStart();
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

    public static void StartMovementExecutionSubPhase(string name)
    {
        StartTemporarySubPhase(name, new MovementExecutionSubPhase());
    }

    private static void StartTemporarySubPhase(string name, GenericSubPhase subPhase)
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

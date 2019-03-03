using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;

public class PhaseEvents
{
    public delegate void EventHandler();
    public event EventHandler OnGameStart;
    public event EventHandler OnSetupStart;
    public event EventHandler OnSetupEnd;
    public event EventHandler OnRoundStart;
    public event EventHandler OnInitiativeSelection;
    public event EventHandler OnPlanningPhaseStart;
    public event EventHandler OnSystemsPhaseStart;
    public event EventHandler OnActivationPhaseStart;
    public event EventHandler BeforeActionSubPhaseStart;
    public event EventHandler OnActionSubPhaseStart;
    public event EventHandler OnActivationPhaseEnd_NoTriggers;
    public event EventHandler OnActivationPhaseEnd_Triggers;
    public event EventHandler OnCombatPhaseStart_NoTriggers;
    public event EventHandler OnCombatPhaseStart_Triggers;
    public event EventHandler OnCombatPhaseEnd_NoTriggers;
    public event EventHandler OnCombatPhaseEnd_Triggers;
    public event EventHandler OnEndPhaseStart_NoTriggers;
    public event EventHandler OnEndPhaseStart_Triggers;
    public event EventHandler OnRoundEnd;
    public event EventHandler OnGameEnd;
    public event EventHandler OnEngagementInitiativeChanged;

    public bool HasOnActivationPhaseEnd { get { return OnActivationPhaseEnd_Triggers != null; } }
    public bool HasOnCombatPhaseStartEvents { get { return OnCombatPhaseStart_Triggers != null; } }
    public bool HasOnCombatPhaseEndEvents { get { return OnCombatPhaseEnd_Triggers != null; } }
    public bool HasOnEndPhaseStartEvents { get { return OnEndPhaseStart_Triggers != null; } }

    // TRIGGERS

    public void CallRoundStartTrigger(Action callback)
    {
        if (OnRoundStart != null) OnRoundStart();

        Triggers.ResolveTriggers(TriggerTypes.OnRoundStart, callback);
    }

    public void CallGameStartTrigger(Action callBack)
    {
        if (OnGameStart != null) OnGameStart();

        foreach (var ship in Roster.AllShips)
        {
            ship.Value.CallOnGameStart();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnGameStart, callBack);
    }

    public void CallSetupStart(Action callBack)
    {
        if (OnSetupStart != null) OnSetupStart();

        Triggers.ResolveTriggers(TriggerTypes.OnSetupStart, callBack);
    }

    public void CallSetupEnd(Action callBack)
    {
        if (OnSetupEnd != null) OnSetupEnd();

        Triggers.ResolveTriggers(TriggerTypes.OnSetupEnd, callBack);
    }

    public void CallInitialiveSelection(Action callBack)
    {
        if (OnInitiativeSelection != null) OnInitiativeSelection();

        Triggers.ResolveTriggers(TriggerTypes.OnInitiativeSelection, callBack);
    }

    public void CallPlanningPhaseTrigger(Action callback)
    {
        if (OnPlanningPhaseStart != null) OnPlanningPhaseStart();

        Triggers.ResolveTriggers(TriggerTypes.OnPlanningSubPhaseStart, callback);
    }

    public void CallActivationPhaseStartTrigger()
    {
        if (OnActivationPhaseStart != null) OnActivationPhaseStart();
        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnActivationPhaseStart();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnActivationPhaseStart, delegate () { Phases.FinishSubPhase(typeof(ActivationStartSubPhase)); });
    }

    public void CallSystemsPhaseStartTrigger()
    {
        if (OnSystemsPhaseStart != null) OnSystemsPhaseStart();
        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnSystemsPhaseStart();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnSystemsPhaseStart, delegate () { Phases.FinishSubPhase(typeof(SystemsStartSubPhase)); });
    }

    public void CallActivationPhaseEndTrigger()
    {
        if (OnActivationPhaseEnd_NoTriggers != null) OnActivationPhaseEnd_NoTriggers();
        if (OnActivationPhaseEnd_Triggers != null) OnActivationPhaseEnd_Triggers();

        Triggers.ResolveTriggers(TriggerTypes.OnActivationPhaseEnd, delegate () { Phases.FinishSubPhase(typeof(ActivationEndSubPhase)); });
    }


    public void CallCombatPhaseStartTrigger()
    {
        if (OnCombatPhaseStart_NoTriggers != null) OnCombatPhaseStart_NoTriggers();
        if (OnCombatPhaseStart_Triggers != null) OnCombatPhaseStart_Triggers();

        Triggers.ResolveTriggers(TriggerTypes.OnCombatPhaseStart, delegate () { Phases.FinishSubPhase(typeof(CombatStartSubPhase)); });
    }

    public void CallCombatPhaseEndTrigger()
    {
        if (OnCombatPhaseEnd_NoTriggers != null) OnCombatPhaseEnd_NoTriggers();
        if (OnCombatPhaseEnd_Triggers != null) OnCombatPhaseEnd_Triggers();

        Triggers.ResolveTriggers(TriggerTypes.OnCombatPhaseEnd, delegate () { Phases.FinishSubPhase(typeof(CombatEndSubPhase)); });
    }

    public void CallEndPhaseTrigger(Action callBack)
    {
        if (OnEndPhaseStart_NoTriggers != null) OnEndPhaseStart_NoTriggers();
        if (OnEndPhaseStart_Triggers != null) OnEndPhaseStart_Triggers();

        Triggers.ResolveTriggers(TriggerTypes.OnEndPhaseStart, callBack);
    }

    public void CallRoundEndTrigger(Action callback)
    {
        if (OnRoundEnd != null) OnRoundEnd();

        foreach (var shipHolder in Roster.AllShips)
        {
            shipHolder.Value.CallOnRoundEnd();
        }

        Triggers.ResolveTriggers(TriggerTypes.OnRoundEnd, callback);
    }

    public void CallBeforeActionSubPhaseTrigger()
    {
        if (BeforeActionSubPhaseStart != null) BeforeActionSubPhaseStart();
    }

    public void CallOnActionSubPhaseTrigger()
    {
        if (OnActionSubPhaseStart != null) OnActionSubPhaseStart();
        Selection.ThisShip.CallOnActionSubPhaseStart();

        Triggers.ResolveTriggers(TriggerTypes.OnActionSubPhaseStart, delegate () { Phases.FinishSubPhase(typeof(ActionSubPhase)); });
    }

    public void CallEndGame()
    {
        if (OnGameEnd != null) OnGameEnd();
    }

    public void CallEngagementInitiativeChanged(Action callback)
    {
        if (OnEngagementInitiativeChanged != null) OnEngagementInitiativeChanged();

        Triggers.ResolveTriggers(TriggerTypes.OnEngagementInitiativeChanged, callback);
    }
}

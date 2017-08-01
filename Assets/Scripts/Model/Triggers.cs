using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

public enum TriggerTypes
{
    None,
    OnSetupPhaseStart,
    OnShipMovementExecuted,
    OnShipMovementFinish,
    OnPositionFinish,
    OnActionSubPhaseStart,
    OnCombatPhaseStart,
    OnFaceupCritCardReadyToBeDealt,
    OnDamageCardIsDealt,
    OnCritDamageCardIsDealt
}

public class Trigger
{
    public string Name;
    public Players.PlayerNo TriggerOwner;
    public TriggerTypes triggerType;
    public EventHandler eventHandler;
    public object sender;
    public EventArgs eventArgs;

    public void Fire()
    {
        eventHandler(sender, eventArgs);
    }
}

public static partial class Triggers
{

    private static List<StackLevel> triggersStackList = new List<StackLevel>();
    private static Trigger currentTrigger;
    private static List<Trigger> currentTriggersList;

    // SUBCLASS

    private class StackLevel
    {
        private List<Trigger> triggers = new List<Trigger>();
        public Action CallBack;

        public int GetSize()
        {
           return triggers.Count;
        }

        public bool Empty()
        {
            return GetSize() == 0;
        }

        public Trigger GetFirst()
        {
            return triggers[0];
        }

        public void AddTrigger(Trigger trigger)
        {
            triggers.Add(trigger);
        }

        public void RemoveTrigger(Trigger trigger)
        {
            triggers.Remove(trigger);
        }

        public List<Trigger> GetTriggersByPlayer(Players.PlayerNo playerNo)
        {
            return triggers.Where(n => n.TriggerOwner == playerNo).ToList<Trigger>();
        }

    }

    // MAIN

    public static void RegisterTrigger(Trigger trigger)
    {
        Debug.Log("Level of stack is " + triggersStackList.Count);
        if (triggersStackList.Count == 0)
        {
            CreateTriggerInNewLevel(trigger);
        }
        else
        {
            AddTriggerToCurrentStackLevel(trigger);
        }
    }

    public static void ResolveTriggersByType(TriggerTypes triggerType, Action callBack = null)
    {
        Debug.Log("ResolveTriggersByType: " + triggerType.ToString() + ", level " + (triggersStackList.Count));

        StackLevel currentStackLevel = GetCurrentStackLevel();

        if ((currentStackLevel != null) && (!currentStackLevel.Empty()))
        {
            if (callBack != null)
            {
                currentStackLevel.CallBack = callBack;
            }

            CreateNewLevelOfStack();
            GetCurrentStackLevel().CallBack = delegate () { ResolveTriggersByType(triggerType); };

            List<Trigger> playerTriggers = currentStackLevel.GetTriggersByPlayer(Phases.PlayerWithInitiative);
            if (playerTriggers.Count > 0)
            {
                ResolveTriggersByTypeAndPlayer(currentStackLevel, triggerType, Phases.PlayerWithInitiative);
            }
            else
            {
                ResolveTriggersByTypeAndPlayer(currentStackLevel, triggerType, Roster.AnotherPlayer(Phases.PlayerWithInitiative));
            }
        }
        else
        {
            if (currentStackLevel == null)
            {
                CreateNewLevelOfStack();
                GetCurrentStackLevel().CallBack = callBack;
            }
            DoCallBack();
        }
    }

    private static void ResolveTriggersByTypeAndPlayer(StackLevel currentStackLevel, TriggerTypes triggerType, Players.PlayerNo playerNo)
    {
        List<Trigger> playerTriggers = currentStackLevel.GetTriggersByPlayer(playerNo);

        if (playerTriggers.Count != 0)
        {
            if (playerTriggers.Count == 1)
            {
                currentTrigger = playerTriggers[0];
                currentStackLevel.RemoveTrigger(currentTrigger);
                currentTrigger.Fire();
            }
            else
            {
                throw new NotImplementedException();

                //currentTriggersList = playerTriggers;
                // Run Decision Subphase
            }
        }
    }

    public static void FinishTrigger()
    {
        StackLevel currentStackLevel = GetCurrentStackLevel();
        currentStackLevel.RemoveTrigger(currentTrigger);
        if (currentStackLevel.Empty())
        {
            DoCallBack();
        }
        else
        {
            throw new NotImplementedException();

            // Run Decision Subphase
        }
    }

    private static void DoCallBack()
    {
        StackLevel currentStackLevel = GetCurrentStackLevel();
        Action callBack = currentStackLevel.CallBack;
        RemoveLastLevelOfStack();
        callBack();
    }

    // SUB

    private static void RemoveLastLevelOfStack()
    {
        Debug.Log("New level of stack removed: " + (triggersStackList.Count));
        triggersStackList.Remove(triggersStackList[triggersStackList.Count - 1]);
    }

    private static StackLevel GetCurrentStackLevel()
    {
        StackLevel result = null;
        if (triggersStackList.Count > 0)
        {
            result = triggersStackList[triggersStackList.Count - 1];
        }
        return result;
    }

    private static TriggerTypes GetCurrentStackLevelTriggerType()
    {
        return triggersStackList[triggersStackList.Count - 1].GetFirst().triggerType;
    }

    private static void CreateTriggerInNewLevel(Trigger trigger)
    {
        CreateNewLevelOfStack();
        AddTriggerToCurrentStackLevel(trigger);
    }

    private static void AddTriggerToCurrentStackLevel(Trigger trigger)
    {
        Debug.Log("Trigger is added: " + trigger.Name);
        triggersStackList[triggersStackList.Count - 1].AddTrigger(trigger);
    }

    private static void CreateNewLevelOfStack()
    {
        Debug.Log("New level of stack created: " + (triggersStackList.Count + 1));
        triggersStackList.Add(new StackLevel());
    }

    // SUBPHASE

    private class NewTriggersOrderSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Select a trigger to resolve";

            foreach (var trigger in currentTriggersList)
            {
                AddDecision(trigger.Name, delegate {
                    Phases.FinishSubPhase(this.GetType());
                    trigger.Fire();
                });
            }

            defaultDecision = GetDecisions().First().Key;
        }

    }

}


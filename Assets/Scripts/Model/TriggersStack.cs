using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

public enum NewTriggerTypes
{
    None,
    OnSetupPhaseStart
}

public class NewTrigger
{
    public string Name;
    public Players.PlayerNo TriggerOwner;
    public NewTriggerTypes triggerType;
    public EventHandler eventHandler;
    public object sender;
    public EventArgs eventArgs;

    public void Fire()
    {
        eventHandler(sender, eventArgs);
    }
}

public static partial class TriggersStack
{

    private static List<StackLevel> triggersStackList = new List<StackLevel>();
    private static NewTrigger currentTrigger;
    private static List<NewTrigger> currentTriggersList;

    // SUBCLASS

    private class StackLevel
    {
        private List<NewTrigger> triggers = new List<NewTrigger>();
        public Action CallBack;

        public int GetSize()
        {
           return triggers.Count;
        }

        public bool Empty()
        {
            return GetSize() == 0;
        }

        public NewTrigger GetFirst()
        {
            return triggers[0];
        }

        public void AddTrigger(NewTrigger trigger)
        {
            triggers.Add(trigger);
        }

        public void RemoveTrigger(NewTrigger trigger)
        {
            triggers.Remove(trigger);
        }

        public List<NewTrigger> GetTriggersByPlayer(Players.PlayerNo playerNo)
        {
            return triggers.Where(n => n.TriggerOwner == playerNo).ToList<NewTrigger>();
        }

    }

    // MAIN

    public static void RegisterTrigger(NewTrigger trigger)
    {
        if (triggersStackList.Count == 0)
        {
            CreateTriggerInNewLevel(trigger);
        }
        else
        {
            AddTriggerToCurrentStackLevel(trigger);
        }
    }

    public static void ResolveTriggersByType(NewTriggerTypes triggerType, Action callBack = null)
    {
        Debug.Log("ResolveTriggersByType, level " + (triggersStackList.Count));

        StackLevel currentStackLevel = GetCurrentStackLevel();
        Debug.Log("Size: " + currentStackLevel.GetSize());

        if (callBack != null)
        {
            currentStackLevel.CallBack = callBack;
        }

        if (!currentStackLevel.Empty())
        {
            CreateNewLevelOfStack();
            GetCurrentStackLevel().CallBack = delegate () { ResolveTriggersByType(triggerType); };

            List<NewTrigger> playerTriggers = currentStackLevel.GetTriggersByPlayer(Phases.PlayerWithInitiative);
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
            DoCallBack();
        }
    }

    private static void ResolveTriggersByTypeAndPlayer(StackLevel currentStackLevel, NewTriggerTypes triggerType, Players.PlayerNo playerNo)
    {
        List<NewTrigger> playerTriggers = currentStackLevel.GetTriggersByPlayer(playerNo);

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
        Debug.Log("Empty, callback time");
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
        return triggersStackList[triggersStackList.Count - 1];
    }

    private static NewTriggerTypes GetCurrentStackLevelTriggerType()
    {
        return triggersStackList[triggersStackList.Count - 1].GetFirst().triggerType;
    }

    private static void CreateTriggerInNewLevel(NewTrigger trigger)
    {
        CreateNewLevelOfStack();
        AddTriggerToCurrentStackLevel(trigger);
    }

    private static void AddTriggerToCurrentStackLevel(NewTrigger trigger)
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


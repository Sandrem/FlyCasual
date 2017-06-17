using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

public enum TriggerTypes
{
    OnShipMovementFinish
}

public static partial class Triggers
{
    class Trigger
    {
        public string Name { get; private set; }
        public TriggerTypes TriggerType { get; private set; }
        public EventHandler TriggerExecution { get; private set; }
        public int Id { get; private set; }

        public Trigger(string name, TriggerTypes triggerType, EventHandler triggerExecution)
        {
            Name = name;
            TriggerType = triggerType;
            TriggerExecution = triggerExecution;
            Id = counter++;
        }
    };

    private static int counter = 0;
    private static bool empty;

    public static bool Empty
    {
        get { return simultaneousTriggers.Count == 0; }
    }


    static Dictionary<int, Trigger> simultaneousTriggers = new Dictionary<int, Trigger>();

    public static void AddTrigger(string name, TriggerTypes triggerType, EventHandler triggerExecution)
    {
        Debug.Log("Trigger \"" + name + "\" is registered. Id " + counter + ". Active: " + (simultaneousTriggers.Count+1));
        simultaneousTriggers.Add(counter, new Trigger(name, triggerType, triggerExecution));
    }

    public static void RemoveTrigger(int id)
    {
        Debug.Log("Trigger \"" + simultaneousTriggers[id].Name + "\" is unregistered. Id " + id + ". Active: " + (simultaneousTriggers.Count-1));
        simultaneousTriggers.Remove(id);
    }

    public static IEnumerator ResolveAllTriggers(TriggerTypes triggerType)
    {
        while (!Triggers.Empty)
        {
            Debug.Log("Call trigger!");
            yield return Triggers.CallTrigger(triggerType);
        }
    }

    private static IEnumerator CallTrigger(TriggerTypes triggerType)
    {
        var rawResults =
            from n in simultaneousTriggers
            where n.Value.TriggerType == triggerType
            select n;
        Dictionary<int, Trigger> results = rawResults.ToDictionary(n => n.Key, n => n.Value);

        Debug.Log("Trigger + \"" + triggerType + "\" is called. Subscribed by: " + results.Count);
        if (results.Count == 1) {
            RemoveTrigger(results.First().Value.Id);
            results.First().Value.TriggerExecution.Invoke(null, null);
        }
        else if (results.Count > 1)
        {
            Debug.Log("Show windows with results: " + results.Count);
            Phases.StartTemporarySubPhase("Triggers Order", typeof(TriggersOrderSubPhase));
            yield return Phases.WaitForTemporarySubPhasesFinish();
        }
    }

    private class TriggersOrderSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            int counter = 2;
            infoText = "Select a trigger to resolve";

            foreach (var trigger in simultaneousTriggers)
            {
                string name = trigger.Value.Name;
                while (decisions.ContainsKey(name))
                {
                    name = trigger.Value.Name + " #" + counter++;
                }
                decisions.Add(name, delegate {
                    Phases.FinishSubPhase(this.GetType());
                    RemoveTrigger(trigger.Value.Id);
                    trigger.Value.TriggerExecution.Invoke(null, null);
                });
            }

            defaultDecision = decisions.First().Key;
        }

    }

}


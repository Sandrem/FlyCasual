using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SubPhases;
using GameModes;

public static class SwarmManager
{
    public static bool IsActive;

    public static void CheckActivation()
    {
        //Debug.Log(!IsActive + " " + (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase)).ToString() + " " + (Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).GetType() == typeof(Players.HumanPlayer)).ToString());
        if (Phases.CurrentSubPhase == null) return;

        if (!IsActive && Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase) && Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).GetType() == typeof(Players.HumanPlayer))
        {
            if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.A)))
            {
                Activate();
            }
        }
    }

    private static void Activate()
    {
        UI.HideNextButton();

        IsActive = true;

        Triggers.RegisterTrigger(new Trigger {
            Name = "Swarm manager",
            TriggerType = TriggerTypes.OnAbilityDirect,
            TriggerOwner = Phases.CurrentSubPhase.RequiredPlayer,
            EventHandler = ShowSwarmManagerWindow
        });

        Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate { Phases.FinishSubPhase(typeof(SwarmManagerSubPhase)); });
    }

    private static void ShowSwarmManagerWindow(object sender, EventArgs e)
    {
        Phases.StartTemporarySubPhaseNew("Swarm Manager", typeof(SwarmManagerSubPhase), delegate { });
        DirectionsMenu.ShowForAll(AssignManeuverToAllShips, AnyShipHasManeuver);
    }

    private static void AssignManeuverToAllShips(string maneuverCode)
    {
        IsActive = false;

        GameMode.CurrentGameMode.SetSwarmManagerManeuver(maneuverCode);

        Triggers.FinishTrigger();
    }

    public static void SetManeuver(string maneuverCode)
    {
        foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships)
        {
            if (shipHolder.Value.HasManeuver(maneuverCode))
            {
                shipHolder.Value.SetAssignedManeuver(ShipMovementScript.MovementFromString(maneuverCode, shipHolder.Value));
                Roster.HighlightShipOff(shipHolder.Value);
            }
        }

        if (Roster.AllManuversAreAssigned(Phases.CurrentPhasePlayer))
        {
            UI.ShowNextButton();
            UI.HighlightNextButton();
        }
    }

    private static bool AnyShipHasManeuver(string maneuverCode)
    {
        bool result = false;

        foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships)
        {
            if (shipHolder.Value.HasManeuver(maneuverCode))
            {
                result = true;
                break;
            }
        }

        return result;
    }

}

namespace SubPhases
{
    public class SwarmManagerSubPhase: GenericSubPhase
    {
        public override void Start()
        {
            Name = "Swarm Manager";
            IsTemporary = true;
            UpdateHelpInfo();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }
    }
}
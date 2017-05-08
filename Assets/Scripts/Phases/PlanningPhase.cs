using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanningPhase : GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Phase = Phases.Planning;
        SubPhase = SubPhases.AssignManeuvers;
        Game.UI.AddTestLogEntry("Planning phase");

        RequiredPlayer = Game.PhaseManager.PlayerWithInitiative;
        RequiredPilotSkill = GetStartingPilotSkill();

        UpdateHelpInfo();

        Game.PhaseManager.CallPlanningPhaseTrigger();
    }

    public override void NextSubPhase()
    {
        SubPhase = SubPhases.AssignManeuvers;

        if (Game.Roster.AllManuersAreAssigned(PlayerToInt(RequiredPlayer)))
        {
            if (RequiredPlayer == Game.PhaseManager.PlayerWithInitiative)
            {
                RequiredPlayer = AnotherPlayer(RequiredPlayer);
                UpdateHelpInfo();
                //Debug.Log(" - " + CurrentSubPhase + " " + PilotSkillSubPhasePlayer);
            }
            else
            {
                NextPhase();
            }
        }

    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new ActivationPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
    }

    public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
    {
        bool result = false;
        if (ship.PlayerNo == RequiredPlayer)
        {
            result = true;
        }
        else
        {
            Game.UI.ShowError("Ship cannot be selected: Wrong player");
        }
        return result;
    }

}

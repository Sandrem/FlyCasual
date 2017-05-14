using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationSubPhase: GenericSubPhase
{

    public override void StartSubPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Name = "Activation SubPhase";

        Game.UI.AddTestLogEntry(Name);

        Dictionary<int, Player> pilots = Game.Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
        foreach (var pilot in pilots)
        {
            RequiredPilotSkill = pilot.Key;
            RequiredPlayer = pilot.Value;
            UpdateHelpInfo();
        }
    }

    public override void NextSubPhase()
    {
        Game.PhaseManager.CurrentSubPhase = new ActionSubphase();
        Game.PhaseManager.CurrentSubPhase.PreviousSubPhase = this;
        Game.PhaseManager.CurrentSubPhase.RequiredPilotSkill = RequiredPilotSkill;
        Game.PhaseManager.CurrentSubPhase.RequiredPlayer = RequiredPlayer;
        Game.PhaseManager.CurrentSubPhase.StartSubPhase();
    }

    public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
    {
        bool result = false;

        if ((ship.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill))
        {
            result = true;
        }
        else
        {
            Game.UI.ShowError("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
        }

        /*if (SubPhase == SubPhases.PerformAction)
        {
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
        }*/
        return result;
    }

    public override int CountActiveButtons(Ship.GenericShip ship)
    {
        int result = 0;
        if (Game.Selection.ThisShip.AssignedManeuver != null)
        {
            Game.UI.panelContextMenu.transform.Find("MovePerformButton").gameObject.SetActive(true);
            result++;
        }
        else
        {
            Game.UI.ShowError("This ship has already executed his maneuver");
        };
        return result;
    }

}

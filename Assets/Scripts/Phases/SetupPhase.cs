using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetupPhase: GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Phase = Phases.Setup;
        SubPhase = SubPhases.Setup;
        Game.UI.AddTestLogEntry("Setup phase");

        RequiredPilotSkill = GetStartingPilotSkill();

        Game.PhaseManager.CallSetupPhaseTrigger();

        NextSubPhase();
    }

    public override void NextSubPhase()
    {
        NextSubPhaseCommon(Sorting.Asc);

        Game.Position.HighlightStartingZones();
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new PlanningPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
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
            Game.UI.ShowError("Ship cannot be selected:\n Need " + Game.PhaseManager.CurrentPhase.RequiredPlayer + " and pilot skill " + Game.PhaseManager.CurrentPhase.RequiredPilotSkill);
        }
        return result;
    }

}

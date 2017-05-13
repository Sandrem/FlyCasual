using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActivationPhase : GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Phase = Phases.Activation;
        SubPhase = SubPhases.AssignManeuvers;
        Game.UI.AddTestLogEntry("Activation phase");

        RequiredPilotSkill = GetStartingPilotSkill();

        Game.PhaseManager.CallActivationPhaseTrigger();

        NextSubPhase();
    }

    public override void NextSubPhase()
    {
        switch (SubPhase)
        {
            case SubPhases.PerformManeuver:
                SubPhase = SubPhases.PerformAction;
                if (!Game.Selection.ThisShip.IsBumped)
                {
                    Game.UI.ActionsPanel.ShowActionsPanel(true);
                }
                else
                {
                    Game.Selection.ThisShip.IsBumped = false;
                    Game.UI.ShowError("Collision: Skips \"Perform Action\" step");
                    Game.UI.AddTestLogEntry("Collision: Skips \"Perform Action\" step");
                    NextSubPhase();
                }
                break;
            case SubPhases.AssignManeuvers:
                Game.Selection.DeselectAllShips();
                SubPhase = SubPhases.PerformManeuver;
                NextSubPhaseCommon(Sorting.Asc);
                break;
            case SubPhases.PerformAction:
                Game.Selection.DeselectAllShips();
                SubPhase = SubPhases.PerformManeuver;
                NextSubPhaseCommon(Sorting.Asc);
                break;
            default:
                break;
        }
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new CombatPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
    }

    public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
    {
        bool result = false;
        if (SubPhase == SubPhases.PerformManeuver)
        {
            if ((ship.PlayerNo ==RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill))
            {
                result = true;
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
            }
        }

        if (SubPhase == SubPhases.PerformAction)
        {
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
        }
        return result;
    }

}

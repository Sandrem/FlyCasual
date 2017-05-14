using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningSubPhase : GenericSubPhase
{

    public override void StartSubPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Name = "Planning SubPhase";

        RequiredPlayer = Game.PhaseManager.PlayerWithInitiative;
        RequiredPilotSkill = GetStartingPilotSkill();

        Game.UI.AddTestLogEntry(Name);

        UpdateHelpInfo();
    }

    public override void NextSubPhase()
    {
        if (Game.Roster.AllManuersAreAssigned(RequiredPlayer))
        {
            if (RequiredPlayer == Game.PhaseManager.PlayerWithInitiative)
            {
                RequiredPlayer = AnotherPlayer(RequiredPlayer);
                UpdateHelpInfo();
            }
            else
            {
                Game.PhaseManager.CurrentPhase.NextPhase();
            }
        }
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

    public override int CountActiveButtons(Ship.GenericShip ship)
    {
        int result = 0;
        Game.UI.panelContextMenu.transform.Find("MoveMenuButton").gameObject.SetActive(true);
        result++;
        return result;
    }

}

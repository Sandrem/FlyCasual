using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatPhase : GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Phase = Phases.Combat;
        SubPhase = SubPhases.PerformAttack;
        Game.UI.AddTestLogEntry("Combat phase");

        RequiredPilotSkill = GetStartingPilotSkill();

        Game.PhaseManager.CallCombatPhaseTrigger();

        NextSubPhase();
    }

    public override void NextSubPhase()
    {
        SubPhase = SubPhases.PerformAttack;

        NextSubPhaseCommon(Sorting.Desc);
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new EndPhase();
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
            Game.UI.ShowError("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
        }
        return result;
    }

    protected override int GetStartingPilotSkill()
    {
        return PILOTSKILL_MAX + 1;
    }
}

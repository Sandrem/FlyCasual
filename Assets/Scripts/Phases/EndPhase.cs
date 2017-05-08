using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndPhase : GenericPhase
{
    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Phase = Phases.End;
        SubPhase = SubPhases.None;
        Game.UI.AddTestLogEntry("End phase");

        RequiredPlayer = Player.None;

        Game.PhaseManager.CallEndPhaseTrigger();

        NextSubPhase();
    }

    public override void NextSubPhase()
    {
        SubPhase = SubPhases.None;

        NextPhase();
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new PlanningPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
    }

}

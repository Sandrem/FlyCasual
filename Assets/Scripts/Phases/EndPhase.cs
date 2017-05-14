using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPhase : GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Name = "Setup Phase";

        Game.PhaseManager.CurrentSubPhase = new EndSubPhase();
        Game.PhaseManager.CurrentSubPhase.StartSubPhase();

        Game.PhaseManager.CallEndPhaseTrigger();
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new PlanningPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
    }

}

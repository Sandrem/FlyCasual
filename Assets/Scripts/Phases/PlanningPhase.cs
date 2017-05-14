using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningPhase : GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Name = "Planning Phase";

        Game.PhaseManager.CurrentSubPhase = new PlanningSubPhase();
        Game.PhaseManager.CurrentSubPhase.StartSubPhase();

        Game.PhaseManager.CallSetupPhaseTrigger();
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();

        Game.PhaseManager.CurrentPhase = new ActivationPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
    }

}

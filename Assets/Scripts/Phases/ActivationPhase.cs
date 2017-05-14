using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPhase : GenericPhase
{

    public override void StartPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Name = "Activation Phase";

        Game.PhaseManager.CurrentSubPhase = new ActivationSubPhase();
        Game.PhaseManager.CurrentSubPhase.StartSubPhase();

        Game.PhaseManager.CallActivationPhaseTrigger();
    }

    public override void NextPhase()
    {
        Game.Selection.DeselectAllShips();
        Game.PhaseManager.CurrentPhase = new CombatPhase();
        Game.PhaseManager.CurrentPhase.StartPhase();
    }

}

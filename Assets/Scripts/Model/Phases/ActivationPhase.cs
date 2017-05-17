using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace Phases
{

    public class ActivationPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Activation Phase";

            Game.Phases.CurrentSubPhase = new ActivationSubPhase();
            Game.Phases.CurrentSubPhase.StartSubPhase();

            Game.Phases.CallActivationPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();
            Game.Phases.CurrentPhase = new CombatPhase();
            Game.Phases.CurrentPhase.StartPhase();
        }

    }

}

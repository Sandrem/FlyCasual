using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class ActivationPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Activation Phase";

            Phases.CurrentSubPhase = new ActivationSubPhase();
            Phases.CurrentSubPhase.StartSubPhase();

            Phases.CallActivationPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();
            Phases.CurrentPhase = new CombatPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}

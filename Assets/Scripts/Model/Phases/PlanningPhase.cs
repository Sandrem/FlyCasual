using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace Phases
{

    public class PlanningPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Planning Phase";

            Game.Phases.CurrentSubPhase = new PlanningSubPhase();
            Game.Phases.CurrentSubPhase.StartSubPhase();

            Game.Phases.CallSetupPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Game.Phases.CurrentPhase = new ActivationPhase();
            Game.Phases.CurrentPhase.StartPhase();
        }

    }

}
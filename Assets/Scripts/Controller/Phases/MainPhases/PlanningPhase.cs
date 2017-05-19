using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class PlanningPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Planning Phase";

            Phases.CurrentSubPhase = new PlanningSubPhase();
            Phases.CurrentSubPhase.StartSubPhase();

            Phases.CallSetupPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Phases.CurrentPhase = new ActivationPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}
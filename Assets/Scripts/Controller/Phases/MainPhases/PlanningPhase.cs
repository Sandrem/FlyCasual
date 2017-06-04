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
            Phases.CallRoundStartTrigger();

            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Planning Phase";

            Phases.CurrentSubPhase = new PlanningSubPhase();
            Phases.CurrentSubPhase.Start();

            Phases.CallPlanningPhaseTrigger();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            Phases.CurrentPhase = new ActivationPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}
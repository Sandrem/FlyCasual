using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class SetupPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Setup Phase";

            Phases.CurrentSubPhase = new SetupSubPhase();
            Phases.CurrentSubPhase.StartSubPhase();

            Phases.CallSetupPhaseTrigger();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            Phases.CurrentPhase = new PlanningPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace Phases
{

    public class SetupPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Setup Phase";

            Game.Phases.CurrentSubPhase = new SetupSubPhase();
            Game.Phases.CurrentSubPhase.StartSubPhase();

            Game.Phases.CallSetupPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Game.Phases.CurrentPhase = new PlanningPhase();
            Game.Phases.CurrentPhase.StartPhase();
        }

    }

}
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

            Game.PhaseManager.CurrentSubPhase = new SetupSubPhase();
            Game.PhaseManager.CurrentSubPhase.StartSubPhase();

            Game.PhaseManager.CallSetupPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Game.PhaseManager.CurrentPhase = new PlanningPhase();
            Game.PhaseManager.CurrentPhase.StartPhase();
        }

    }

}
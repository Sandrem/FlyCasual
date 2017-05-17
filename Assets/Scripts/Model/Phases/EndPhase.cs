using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace Phases
{

    public class EndPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Setup Phase";

            Game.Phases.CurrentSubPhase = new EndSubPhase();
            Game.Phases.CurrentSubPhase.StartSubPhase();

            Game.Phases.CallEndPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Game.Phases.CurrentPhase = new PlanningPhase();
            Game.Phases.CurrentPhase.StartPhase();
        }

    }

}

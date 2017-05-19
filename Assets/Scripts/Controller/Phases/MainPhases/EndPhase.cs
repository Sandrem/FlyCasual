using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class EndPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Setup Phase";

            Phases.CurrentSubPhase = new EndSubPhase();
            Phases.CurrentSubPhase.StartSubPhase();

            Phases.CallEndPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Phases.CurrentPhase = new PlanningPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace Phases
{

    public class CombatPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Combat Phase";

            Game.Phases.CurrentSubPhase = new CombatSubPhase();
            Game.Phases.CurrentSubPhase.StartSubPhase();

            Game.Phases.CallCombatPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Game.Phases.CurrentPhase = new EndPhase();
            Game.Phases.CurrentPhase.StartPhase();
        }

    }

}

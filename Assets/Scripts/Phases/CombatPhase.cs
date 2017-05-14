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

            Game.PhaseManager.CurrentSubPhase = new CombatSubPhase();
            Game.PhaseManager.CurrentSubPhase.StartSubPhase();

            Game.PhaseManager.CallCombatPhaseTrigger();
        }

        public override void NextPhase()
        {
            Game.Selection.DeselectAllShips();

            Game.PhaseManager.CurrentPhase = new EndPhase();
            Game.PhaseManager.CurrentPhase.StartPhase();
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class CombatPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Name = "Combat Phase";

            Phases.CurrentSubPhase = new CombatSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Initialize();

            Phases.CallCombatPhaseTrigger();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            Phases.CurrentPhase = new EndPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}

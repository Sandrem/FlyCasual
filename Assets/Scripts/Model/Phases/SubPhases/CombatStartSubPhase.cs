using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class CombatStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Combat start";
            UpdateHelpInfo();

            if (DebugManager.DebugPhases) Debug.Log("Combat Start - Started");
        }

        public override void Initialize()
        {
            Phases.CallCombatPhaseStartTrigger();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = new CombatSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip, int mouseKeyIsPressed)
        {
            return false;
        }

    }

}

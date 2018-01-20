using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class CombatEndSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Combat end";
            UpdateHelpInfo();

            if (DebugManager.DebugPhases) Debug.Log("Combat End - Started");
        }

        public override void Initialize()
        {
            Phases.CallCombatPhaseEndTrigger();
        }

        public override void Next()
        {
            UI.HideSkipButton();
            Phases.CurrentPhase.NextPhase();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class ActivationEndSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Activation end";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.Events.CallActivationPhaseEndTrigger();
        }

        public override void Next()
        {
            Phases.NextPhase();
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

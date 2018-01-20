using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class SetupStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Setup start";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.CallGameStartTrigger(Phases.CallSetupPhaseTrigger);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = new SetupSubPhase();
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

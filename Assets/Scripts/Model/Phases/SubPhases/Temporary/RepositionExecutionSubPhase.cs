using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class RepositionExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            base.Start();

            Name = "Barrel Roll";
            IsTemporary = true;
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            UpdateHelpInfo();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

}

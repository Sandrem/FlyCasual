using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class PlanningStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Planning start";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.CallPlanningPhaseTrigger();
            Phases.FinishSubPhase(this.GetType());
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = new PlanningSubPhase();
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

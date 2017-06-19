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
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Setup start";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.CallSetupPhaseTrigger();
            Phases.FinishSubPhase(this.GetType());
        }

        public override void Next()
        {
            FinishPhase();
        }

        public override void FinishPhase()
        {
            Phases.CurrentSubPhase = new SetupSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            return false;
        }

    }

}

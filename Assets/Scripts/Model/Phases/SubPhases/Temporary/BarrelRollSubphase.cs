using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class BarrelRollSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Barrel Roll SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();

            Game.Position.StartBarrelRoll();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
            return result;
        }

    }

}

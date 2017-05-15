using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class BarrelRollSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Barrel Roll SubPhase";
            isTemporary = true;
            Game.UI.AddTestLogEntry(Name);
            UpdateHelpInfo();

            Game.Position.StartBarrelRoll();
        }

        public override void NextSubPhase()
        {
            Game.PhaseManager.CurrentSubPhase = PreviousSubPhase;
            Game.PhaseManager.CurrentSubPhase.NextSubPhase();
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

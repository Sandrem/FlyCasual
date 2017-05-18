using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class FreeActionSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Free Action SubPhase";
            isTemporary = true;
            Game.UI.AddTestLogEntry(Name);
            UpdateHelpInfo();
        }

        public override void NextSubPhase()
        {
            Game.Phases.CurrentSubPhase = PreviousSubPhase;
            Game.Phases.Next();
            Game.Phases.FinishSubPhase(typeof(ActivationSubPhase));
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
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

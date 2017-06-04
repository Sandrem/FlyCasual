using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class FreeActionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Debug.Log("Free Action: Start");
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Free Action SubPhase";
            isTemporary = true;
            Game.UI.AddTestLogEntry(Name);
            UpdateHelpInfo();
        }

        public override void Next()
        {
            Debug.Log("Free Action: Next");
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.Next();
            Phases.FinishSubPhase(typeof(ActivationSubPhase));
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

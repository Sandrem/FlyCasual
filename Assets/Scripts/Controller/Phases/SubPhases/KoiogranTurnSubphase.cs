using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class KoiogranTurnSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Koiogran Turn SubPhase";
            isTemporary = true;
            Game.UI.AddTestLogEntry(Name);
            UpdateHelpInfo();

            Game.Position.StartKoiogranTurn();
        }

        public override void NextSubPhase()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.Next();
            Phases.Next();
            //Phases.FinishSubPhase(typeof(ActivationSubPhase));

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
            return result;
        }

    }

}

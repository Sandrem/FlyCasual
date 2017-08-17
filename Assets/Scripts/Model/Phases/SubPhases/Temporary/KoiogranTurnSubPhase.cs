using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class KoiogranTurnSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Koiogran Turn SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();

            Game.Position.StartKoiogranTurn();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
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

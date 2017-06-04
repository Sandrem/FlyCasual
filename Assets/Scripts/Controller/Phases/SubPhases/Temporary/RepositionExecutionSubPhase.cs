using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class RepositionExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Debug.Log("BR: Start");
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Barrel Roll";
            isTemporary = true;
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            UpdateHelpInfo();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
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

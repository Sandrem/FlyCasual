using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class MovementExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Debug.Log("Move: Start");
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Movement";
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            UpdateHelpInfo();
        }

        public override void Next()
        {
            Debug.Log("So, movement is finished...");

            Debug.Log("Move: Next?");
            if (Phases.CurrentSubPhase.GetType() == this.GetType())
            {
                Debug.Log("Move: Next");
                GenericSubPhase actionSubPhase = new ActionSubPhase();
                actionSubPhase.PreviousSubPhase = Phases.CurrentSubPhase;
                Phases.CurrentSubPhase = actionSubPhase;
                Phases.CurrentSubPhase.Start();
                Phases.CurrentSubPhase.Initialize();
            }
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

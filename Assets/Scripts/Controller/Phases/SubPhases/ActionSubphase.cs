using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Debug.Log("Action: Start");
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Action SubPhase";
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            Game.UI.AddTestLogEntry(Name);
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Debug.Log("Action: Init");

            //BUG: Error During BarrelRoll
            if (!Selection.ThisShip.IsBumped)
            {
                if (!Selection.ThisShip.IsDestroyed)
                {
                    Selection.ThisShip.GenerateAvailableActionsList();
                    Roster.GetPlayer(RequiredPlayer).PerformAction();
                }
                else
                {
                    FinishPhase();
                }
            }
            else
            {
                Selection.ThisShip.IsBumped = false;
                Game.UI.ShowError("Collision: Skips \"Perform Action\" step");
                Game.UI.AddTestLogEntry("Collision: Skips \"Perform Action\" step");
                FinishPhase();
            }
        }

        public override void Next()
        {
            Debug.Log("Action: Next");

            GenericSubPhase activationSubPhase = new ActivationSubPhase();
            Phases.CurrentSubPhase = activationSubPhase;
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.RequiredPilotSkill = RequiredPilotSkill;
            Phases.CurrentSubPhase.RequiredPlayer = RequiredPlayer;

            Phases.CurrentSubPhase.Next();
        }

        public override void FinishPhase()
        {
            Debug.Log("Action: Finish");
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
            return result;
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Action SubPhase";
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.CallOnActionSubPhaseTrigger();

            if (!Selection.ThisShip.IsSkipsActionSubPhase)
            {
                if (!Selection.ThisShip.IsDestroyed)
                {
                    Selection.ThisShip.GenerateAvailableActionsList();
                    Roster.GetPlayer(RequiredPlayer).PerformAction();
                }
                else
                {
                    Next();
                }
            }
            else
            {
                Selection.ThisShip.IsSkipsActionSubPhase = false;
                Next();
            }
        }

        public override void Next()
        {
            Selection.ThisShip.CallAfterActionIsPerformed(this.GetType());

            if (Phases.CurrentSubPhase.GetType() == this.GetType())
            {
                FinishPhase();
            }
        }

        public override void FinishPhase()
        {
            GenericSubPhase activationSubPhase = new ActivationSubPhase();
            Phases.CurrentSubPhase = activationSubPhase;
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.RequiredPilotSkill = RequiredPilotSkill;
            Phases.CurrentSubPhase.RequiredPlayer = RequiredPlayer;

            Phases.CurrentSubPhase.Next();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
            return result;
        }

    }

}

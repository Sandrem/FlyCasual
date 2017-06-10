using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class SelectTargetSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            isTemporary = true;
            UpdateHelpInfo();
            Roster.HighlightShipsFiltered(Roster.AnotherPlayer(Phases.CurrentPhasePlayer), -1);
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
            Game.UI.ShowError("Select enemy ship");
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;

            if (anotherShip.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer)
            {
                if (!Actions.AssignTargetLockToPair(Selection.ThisShip, anotherShip))
                {
                    Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.TargetLockAction));
                    Phases.CurrentSubPhase = PreviousSubPhase;
                    Roster.AllShipsHighlightOff();
                    UpdateHelpInfo();
                    Actions.ShowActionsPanel();
                    return false;
                }
                Next();
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected as target: Friendly ship");
            }

            return result;
        }

    }

}

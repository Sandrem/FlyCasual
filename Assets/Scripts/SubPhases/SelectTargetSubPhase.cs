using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class SelectTargetSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            isTemporary = true;
            Game.UI.AddTestLogEntry(Name);
            UpdateHelpInfo();
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
            Game.UI.ShowError("Select enemy ship");
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;

            if (anotherShip.PlayerNo != Game.PhaseManager.CurrentSubPhase.RequiredPlayer)
            {
                if (!Game.Actions.AssignTargetLockToPair(Game.Selection.ThisShip, anotherShip))
                {
                    Game.Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(Actions.TargetLockAction));
                    Game.PhaseManager.CurrentSubPhase = PreviousSubPhase;
                    UpdateHelpInfo();
                    Game.UI.ActionsPanel.ShowActionsPanel();
                    return false;
                }
                result = true;
                NextSubPhase();
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected as target: Friendly ship");
            }

            return result;
        }

    }

}

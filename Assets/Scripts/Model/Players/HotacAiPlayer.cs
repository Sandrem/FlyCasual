using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class HotacAiPlayer : GenericAiPlayer
    {
        private bool inDebug = false;

        public HotacAiPlayer(): base() {
            Name = "HotAC AI";
        }

        public override void ActivateShip(Ship.GenericShip ship)
        {
            if (inDebug) Debug.Log("=== " + ship.PilotName + " (" + ship.ShipId + ") ===");

            Ship.GenericShip anotherShip = FindNearestEnemyShip(ship, ignoreCollided:true, inArcAndRange:true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship);
            if (inDebug) Debug.Log("Nearest enemy is " + anotherShip.PilotName + " (" + anotherShip.ShipId + ")");

            /*ship.GenerateAvailableActionsList();
            foreach (var action in ship.GetAvailableActionsList())
            {
                if (action.GetType() == typeof(ActionsList.TargetLockAction))
                {
                    Actions.AssignTargetLockToPair(ship, anotherShip);
                    break;
                }
            }*/

            ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
            PerformManeuverOfShip(ship);
        }

        public override void PerformAction()
        {
            bool actionIsPerformed = false;

            if (Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                Selection.ThisShip.RemoveToken(typeof(Tokens.StressToken));
            }
            else if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                actionIsPerformed = TryToCancelCrits();
                if (!actionIsPerformed) actionIsPerformed = TryToGetShot();
                if (!actionIsPerformed) actionIsPerformed = TryFocus();
                if (!actionIsPerformed) actionIsPerformed = TryEvade();
            }

            if (!actionIsPerformed)
            {
                Phases.CurrentSubPhase.callBack();
            }
        }

        private bool TryToCancelCrits()
        {
            return false;
        }

        private bool TryToGetShot()
        {
            return false;
        }

        private bool TryToAvoidShot()
        {
            return false;
        }

        private bool TryFocus()
        {
            if (Actions.HasTarget(Selection.ThisShip))
            {
                foreach (var availableAction in Selection.ThisShip.GetAvailableActionsList())
                {
                    if (availableAction.GetType() == typeof(ActionsList.FocusAction))
                    {
                        availableAction.ActionTake();
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryEvade()
        {
            foreach (var availableAction in Selection.ThisShip.GetAvailableActionsList())
            {
                if (availableAction.GetType() == typeof(ActionsList.EvadeAction))
                {
                    availableAction.ActionTake();
                    return true;
                }
            }
            return false;
        }

    }

}

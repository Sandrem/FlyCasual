using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace RulesList
{
    public class CollisionRules
    {
        static bool RuleIsInitialized = false;

        public CollisionRules()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
                RuleIsInitialized = true;
            }
            Phases.Events.BeforeActionSubPhaseStart += CheckSkipPerformAction;
        }

        public void CheckSkipPerformAction()
        {
            string ShipMessageString = "";
            if (Selection.ThisShip.IsBumped && !Selection.ThisShip.CanPerformActionsWhenBumped)
            {
                ShipMessageString = Selection.ThisShip.PilotInfo.PilotName + " collided into another ship.  Skipping their action subphase.";
                Messages.ShowErrorToHuman(ShipMessageString);
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void AddBump(GenericShip ship1, GenericShip ship2)
        {
            if (!ship1.ShipsBumped.Contains(ship2))
            {
                ship1.ShipsBumped.Add(ship2);
            }

            if (!ship2.ShipsBumped.Contains(ship1))
            {
                ship2.ShipsBumped.Add(ship1);
            }
        }

        public void ClearBumps(GenericShip ship)
        {
            foreach (var bumpedShip in ship.ShipsBumped)
            {
                if (bumpedShip.ShipsBumped.Contains(ship))
                {
                    bumpedShip.ShipsBumped.Remove(ship);
                }
            }
            ship.ShipsBumped = new List<GenericShip>();
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            if ((Selection.ThisShip.IsBumped) && (Selection.ThisShip.ShipsBumped.Contains(Selection.AnotherShip)) && (Selection.AnotherShip.ShipsBumped.Contains(Selection.ThisShip)))
            {
                if (!Selection.ThisShip.CanAttackBumpedTarget(Selection.AnotherShip))
                {
                    stringList.Add("You cannot attack the ship that you are touching.");
                    result = false;
                }
            }
        }

    }
}

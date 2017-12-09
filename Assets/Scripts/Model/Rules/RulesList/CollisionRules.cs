using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class CollisionRules
    {

        public CollisionRules()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.BeforeActionSubPhaseStart += CheckSkipPerformAction;
            Ship.GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
        }

        public void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.IsBumped)
            {
                Messages.ShowErrorToHuman("Collided into ship - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void ClearBumps(Ship.GenericShip ship)
        {
            foreach (var bumpedShip in ship.ShipsBumped)
            {
                if (bumpedShip.ShipsBumped.Contains(ship))
                {
                    bumpedShip.ShipsBumped.Remove(ship);
                }
            }
            ship.ShipsBumped = new List<Ship.GenericShip>();
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            if ((Selection.ThisShip.IsBumped) && (Selection.ThisShip.ShipsBumped.Contains(Selection.AnotherShip)) && (Selection.AnotherShip.ShipsBumped.Contains(Selection.ThisShip)))
            {
                if (!Selection.ThisShip.CanAttackBumpedTarget(Selection.AnotherShip))
                {
                    stringList.Add("Cannot attack ship that you are touching");
                    result = false;
                }
            }
        }

    }
}

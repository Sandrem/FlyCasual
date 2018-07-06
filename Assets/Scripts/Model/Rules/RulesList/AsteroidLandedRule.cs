using Ship;
using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class AsteroidLandedRule
    {
        static bool RuleIsInitialized = false;

        public AsteroidLandedRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
                GenericShip.OnPositionFinishGlobal += InformLandedOnAsteroid;
                RuleIsInitialized = true;
            }
        }

        private void InformLandedOnAsteroid(GenericShip ship)
        {
            if (ship.IsLandedOnObstacle)
            {
                Messages.ShowErrorToHuman("Landed on asteroid, cannot attack");
            }
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            if (Selection.ThisShip.IsLandedOnObstacle)
            {
                stringList.Add("Landed on asteroid - cannot attack");
                result = false;
            }
        }

    }
}

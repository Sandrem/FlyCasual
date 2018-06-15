using Ship;
using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class AsteroidLandedRule
    {

        public AsteroidLandedRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
            GenericShip.OnPositionFinishGlobal += InformLandedOnAsteroid;
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

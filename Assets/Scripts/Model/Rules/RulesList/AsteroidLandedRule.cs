using UnityEngine;

namespace RulesList
{
    public class AsteroidLandedRule
    {

        public AsteroidLandedRule(GameManagerScript game)
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;
            Ship.GenericShip.OnPositionFinishGlobal += InformLandedOnAsteroid;
        }

        private void InformLandedOnAsteroid()
        {
            if (Selection.ThisShip.IsLandedOnObstacle)
            {
                Messages.ShowErrorToHuman("Landed on asteroid, cannot attack");
            }
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if (attacker.IsLandedOnObstacle)
            {
                Messages.ShowErrorToHuman("Landed on asteroid - cannot attack");
                result = false;
            }
        }

    }
}

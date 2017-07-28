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
            Actions.OnCheckTargetIsLegal += CanPerformAttack;
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

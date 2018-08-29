using Ship;

namespace RulesList
{
    public class BonusAttackRule
    {
        public void ResetCanBonusAttack(GenericShip ship)
        {
            ship.CanBonusAttack = true;
        }
    }
}

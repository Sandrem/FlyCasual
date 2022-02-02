using ActionsList;
using Ship;

namespace RulesList
{
    public class DiceModificationRule
    {
        public void PreventRangeZeroOwnModifications(GenericShip ship, GenericAction action, ref bool allowed)
        {
            if (IsAttackerRangeZeroDiceModification(ship, action)) allowed = false;
        }

        public void PreventRangeZeroCompareResultsModifications(GenericAction action, ref bool allowed)
        {
            if (IsAttackerRangeZeroDiceModification(Combat.Attacker, action)) allowed = false;
        }

        private bool IsAttackerRangeZeroDiceModification(GenericShip ship, GenericAction action)
        {
            return Combat.ShotInfo.Range == 0
                && Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.Attacker == ship
                && !action.IsNotRealDiceModification;
        }

    }
}

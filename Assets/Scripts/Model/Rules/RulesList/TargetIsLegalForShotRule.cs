using Ship;

namespace RulesList
{
    public class TargetIsLegalForShotRule
    {
        public bool IsLegal(GenericShip thisShip, GenericShip anotherShip, IShipWeapon weapon, bool isSilent = false)
        {
            bool shipCheckResult = true;
            shipCheckResult = Selection.ThisShip.CallCanPerformAttack(shipCheckResult, null, isSilent);

            bool weaponCheckResult = weapon.IsShotAvailable(anotherShip);

            bool extraAttackFilterResult = false;
            if (Combat.ExtraAttackFilter == null || Combat.ExtraAttackFilter(anotherShip, weapon, isSilent))
            {
                extraAttackFilterResult = true;
            }
            
            return shipCheckResult && weaponCheckResult && extraAttackFilterResult;
        }
    }
}

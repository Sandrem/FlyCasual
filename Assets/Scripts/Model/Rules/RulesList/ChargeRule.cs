using Ship;
using System.Linq;
using Upgrade;
using System.Collections.Generic;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            // Ships
            bool doesRecover = true;
            ship.CallBeforeChargeRecovers(ref doesRecover);
            if (doesRecover)
            {
                ship.RestoreCharges(ship.State.RegensCharges);
            }

            // Upgrades of the ship
            ship.UpgradeBar.GetUpgradesAll().Where(u => u.UpgradeInfo.RegensChargesCount != 0).ToList().ForEach(u =>
            {
                u.State.RestoreCharges(u.UpgradeInfo.RegensChargesCount);
            });

            ship.UpgradeBar.GetSpecialWeaponsAll().ForEach(u =>
            {
                GenericSpecialWeapon specialWeapon = (GenericSpecialWeapon)u;
                if (specialWeapon.WeaponInfo.RegensCharges)
                {
                    u.State.RestoreCharge();
                }
            });            
        }
    }
}

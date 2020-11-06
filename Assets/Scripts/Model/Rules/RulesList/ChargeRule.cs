using Ship;
using System.Linq;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            // Ships
            if (ship.State.RegensCharges && ship.State.Charges < ship.State.MaxCharges) ship.RestoreCharge();

            // Upgrades of the ship
            ship.UpgradeBar.GetUpgradesAll().Where(u => u.UpgradeInfo.RegensChargesCount != 0).ToList().ForEach(u =>
            {
                u.State.RestoreCharges(u.UpgradeInfo.RegensChargesCount);
            });
        }
    }
}

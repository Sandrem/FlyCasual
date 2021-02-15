using Ship;
using System.Linq;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            // Ships
            ship.RestoreCharges(ship.State.RegensCharges);

            // Upgrades of the ship
            ship.UpgradeBar.GetUpgradesAll().Where(u => u.UpgradeInfo.RegensChargesCount != 0).ToList().ForEach(u =>
            {
                u.State.RestoreCharges(u.UpgradeInfo.RegensChargesCount);
            });
        }
    }
}

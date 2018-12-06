using Ship;
using System.Linq;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            if (ship.State.RegensCharges && ship.State.Charges < ship.State.MaxCharges) ship.RestoreCharge();

            ship.UpgradeBar.GetUpgradesAll().Where(u => u.UpgradeInfo.RegensCharges && u.State.Charges < u.State.MaxCharges).ToList().ForEach(u =>
            { 
                u.State.RestoreCharge();
            });
        }
    }
}

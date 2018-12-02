using Ship;
using System.Linq;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            if (ship.State.RegensCharges && ship.State.Charges < ship.State.MaxCharges) ship.RestoreCharge();

            ship.UpgradeBar.GetUpgradesAll().Where(u => u.RegensCharges && u.Charges < u.MaxCharges).ToList().ForEach(u =>
            { 
                u.RestoreCharge();
            });
        }
    }
}

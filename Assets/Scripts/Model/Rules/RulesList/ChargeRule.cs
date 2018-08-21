using Ship;
using UnityEngine;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            if (!ship.RegensCharges)
                return;

            if (ship.Charges < ship.MaxCharges) ship.RestoreCharge();

            // We can add regenerating charges for upgrades here.
        }
    }
}

using Ship;
using UnityEngine;

namespace RulesList
{
    public class ChargeRule
    {
        public void RegenerateCharge(GenericShip ship)
        {
            if (ship.Charges < ship.MaxCharges) ship.Charges++;

            // We can add regenerating charges for upgrades here.
        }
    }
}

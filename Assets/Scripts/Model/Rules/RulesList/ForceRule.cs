using Ship;
using UnityEngine;

namespace RulesList
{
    public class ForceRule
    {

        public void RegenerateForce(GenericShip ship)
        {
            if (ship.State.Force < ship.State.MaxForce
                && ship.IsForceRecurring)
            {
                ship.State.RestoreForce();
            }
        }

        public void AddForceAction(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new ActionsList.ForceAction());
        }
    }
}

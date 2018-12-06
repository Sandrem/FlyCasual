using Ship;
using UnityEngine;

namespace RulesList
{
    public class ForceRule
    {

        public void RegenerateForce(GenericShip ship)
        {
            if (ship.State.Force < ship.State.MaxForce) ship.State.Force++;
        }

        public void AddForceAction(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.ForceAction() { HostShip = ship });
        }
    }
}

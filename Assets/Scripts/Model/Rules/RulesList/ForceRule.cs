using Ship;
using UnityEngine;

namespace RulesList
{
    public class ForceRule
    {

        public void RegenerateForce(GenericShip ship)
        {
            if (ship.Force < ship.MaxForce) ship.Force++;
        }

        public void AddForceAction(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.ForceAction() { Host = ship });
        }
    }
}

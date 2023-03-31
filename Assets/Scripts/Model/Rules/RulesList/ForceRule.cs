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
                bool doesRecover = true;
                ship.CallBeforeForceRecovers(ref doesRecover);
                if (doesRecover)
                {
                    ship.State.RestoreForce(ship.PilotInfo.RegensForce);
                }
            }
        }

        public void AddForceAction(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new ActionsList.ForceAction());
        }
    }
}

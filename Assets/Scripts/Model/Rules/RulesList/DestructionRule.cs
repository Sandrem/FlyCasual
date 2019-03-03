using UnityEngine;
using Ship;
using Tokens;
using ActionsList;
using Players;
using Movement;

namespace RulesList
{
    public class DestructionRule
    {
        public void WhenShipIsRemoved(GenericShip ship)
        {
            Rules.TargetLocks.RegisterRemoveTargetLocksOnDestruction(ship);
            Rules.Collision.ClearBumps(ship);
            ship.DeactivateAllAbilities();
            Roster.RemoveDestroyedShip(ship.GetTag());
        }
    }
}
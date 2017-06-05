using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class HotacAiPlayer : GenericAiPlayer
    {
        private bool inDebug = false;

        public HotacAiPlayer(): base() {
            Name = "HotAC AI";
        }

        public override void ActivateShip(Ship.GenericShip ship)
        {
            if (!ship.IsDestroyed)
            {
                if (inDebug) Debug.Log("=== " + ship.PilotName + " (" + ship.ShipId + ") ===");
                Ship.GenericShip anotherShip = FindNearestEnemyShip(ship);
                if (inDebug) Debug.Log("Nearest enemy is " + anotherShip.PilotName + " (" + anotherShip.ShipId + ")");
                ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
                PerformManeuverOfShip(ship);
            }
        }

    }

}

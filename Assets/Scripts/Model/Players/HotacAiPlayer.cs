using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class HotacAiPlayer : GenericAiPlayer
    {

        public HotacAiPlayer(): base() {
            Name = "HotAC AI";
        }

        public override void ActivateShip(Ship.GenericShip ship)
        {
            if (!ship.IsDestroyed)
            {
                Debug.Log("=== " + ship.PilotName + " (" + ship.ShipId + ") ===");
                Ship.GenericShip anotherShip = FindNearestEnemyShip(ship);
                Debug.Log("Nearest enemy is " + anotherShip.PilotName + " (" + anotherShip.ShipId + ")");
                ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
                PerformManeuverOfShip(ship);
            }
        }

    }

}

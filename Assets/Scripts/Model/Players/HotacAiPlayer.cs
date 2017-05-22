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
            Ship.GenericShip anotherShip = FindNearestEnemyShip(ship);
            ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
            PerformManeuverOfShip(ship);
        }

    }

}

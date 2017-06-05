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
            if (inDebug) Debug.Log("=== " + ship.PilotName + " (" + ship.ShipId + ") ===");

            Ship.GenericShip anotherShip = FindNearestEnemyShip(ship, ignoreCollided:true, inArcAndRange:true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship);
            if (inDebug) Debug.Log("Nearest enemy is " + anotherShip.PilotName + " (" + anotherShip.ShipId + ")");

            ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
            PerformManeuverOfShip(ship);
        }

        public override void PerformAction()
        {
            Debug.Log(Selection.ThisShip.IsSkipsAction);
            if (Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                Selection.ThisShip.RemoveToken(typeof(Tokens.StressToken));
                Debug.Log("I removed stress");
            }
            else if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                Debug.Log("I has actions");
            }

            Phases.Next();
        }

    }

}

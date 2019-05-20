using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using UnityEngine;

namespace Obstacles
{
    public class GasCloud : GenericObstacle
    {
        public GasCloud(string name, string shortName) : base(name, shortName)
        {
            
        }

        public override string GetTypeName => "Gas Cloud";

        public override void OnHit(GenericShip ship)
        {
            // skip action
        }

        public override void OnLanded(GenericShip ship)
        {
            // Nothing
        }

        public override void OnShotObstructed(GenericShip attacker, GenericShip defender)
        {
            // +1 die, blank to evade
        }
    }
}

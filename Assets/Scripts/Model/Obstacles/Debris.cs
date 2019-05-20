using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Obstacles
{
    public class Debris: GenericObstacle
    {
        public Debris(string name, string shortName) : base(name, shortName)
        {
            
        }

        public override string GetTypeName => "Debris";

        public override void OnHit(GenericShip ship)
        {
            // stress
            // roll die
        }

        public override void OnLanded(GenericShip ship)
        {
            // Nothing
        }

        public override void OnShotObstructed(GenericShip attacker, GenericShip defender)
        {
            // +1 die
        }
    }
}

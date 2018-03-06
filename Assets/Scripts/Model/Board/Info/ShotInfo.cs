using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Arcs;
using Upgrade;

namespace Board
{

    public class ShotInfo : RangeInfo
    {
        public bool InShotAngle { get; private set; }
        public bool InArc { get; private set; }
        public Dictionary<BaseArcsType, bool> InArcByType { get; private set; }

        public Dictionary<UpgradeType, bool> CanShootFromWeapon { get; private set; }

        public bool ObstructedByAsteroid { get; private set; }
        public bool ObstructedByMine { get; private set; }

        public ShotInfo(GenericShip ship1, GenericShip ship2) : base(ship1, ship2)
        {
            CheckAngle();
        }

        private void CheckAngle()
        {
            // Check existing min distance vs angles of arc
            // if failed - 
        }

        private void SearchWithRays()
        {
            // Search using min ray and max ray
        }
    }
}



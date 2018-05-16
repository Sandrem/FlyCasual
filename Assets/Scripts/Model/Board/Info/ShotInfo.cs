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

    public class ShotInfo : GenericShipDistanceInfo
    {
        public bool InShotAngle { get; private set; }
        public bool InArc { get; private set; }
        public Dictionary<BaseArcsType, bool> InArcByType { get; private set; }

        public bool ObstructedByAsteroid { get; private set; }
        public bool ObstructedByMine { get; private set; }

        public ShotInfo(GenericShip ship1, GenericShip ship2) : base(ship1, ship2)
        {
        }

        protected override void CheckRange()
        {
            FindNearestDistances(Ship1.ShipBase.GetStandFrontEdgePoins()); //Temporary
            TryFindPerpendicularDistanceA();
            TryFindPerpendicularDistanceB();
            SetFinalMinDistance();

            CheckRequirements();
        }

        private void CheckRequirements()
        {
            if (Range > 3) return;

            float signedAngle = Vector3.SignedAngle(MinDistance.Vector, Ship1.GetFrontFacing(), Vector3.up);
            if (signedAngle < -40 || signedAngle > 40) return;

            InShotAngle = true;
        }
    }
}



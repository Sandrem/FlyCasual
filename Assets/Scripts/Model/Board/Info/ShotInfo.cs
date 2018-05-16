using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Arcs;
using Upgrade;

namespace BoardTools
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
            CheckRays();
        }

        private void CheckRequirements()
        {
            if (Range > 3) return;

            float signedAngle = Vector3.SignedAngle(MinDistance.Vector, Ship1.GetFrontFacing(), Vector3.up);
            if (signedAngle < -40 || signedAngle > 40) return;

            InShotAngle = true;
        }

        private void CheckRays()
        {
            if (InShotAngle) return;

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Ship1.ShipBase.GetStandFrontEdgePoins().First().Value + new Vector3(0, 0.003f, 0), new Vector3(-1, 0.003f, 1), out hitInfo, Board.BoardIntoWorld(3*Board.RANGE_1)))
            {
                Debug.Log(hitInfo.collider.tag + " " + hitInfo.collider.name);
                //hitInfo.point
            }
        }
    }
}



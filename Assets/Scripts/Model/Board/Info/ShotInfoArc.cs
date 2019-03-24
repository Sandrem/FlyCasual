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

    public class ShotInfoArc : GenericShipDistanceInfo
    {
        public bool IsShotAvailable { get; private set; }
        public bool InArc { get; private set; }

        private GenericArc Arc;

        public ShotInfoArc(GenericShip ship1, GenericShip ship2, GenericArc arc) : base(ship1, ship2)
        {
            Arc = arc;

            CheckRange();
        }

        private void CheckRange()
        {
            FindNearestDistances(Ship1.ShipBase.GetGlobalPoints(Arc.Edges));
            TryFindPerpendicularDistanceA();
            TryFindPerpendicularDistanceB();
            SetFinalMinDistance();

            CheckRequirements();
            CheckRays();
        }

        private void CheckRequirements()
        {
            if (Range > 3) return;

            if (Arc.Limits != null && Arc.Limits.Count > 0)
            {
                float signedAngle = (float) Math.Round(Vector3.SignedAngle(MinDistance.Vector, Ship1.GetFrontFacing(), Vector3.down), 2);
                if (Arc.Facing != ArcFacing.Rear && Arc.Facing != ArcFacing.FullRear)
                {
                    if (signedAngle < Arc.Limits.First().Value || signedAngle > Arc.Limits.Last().Value) return;
                }
                else
                {
                    if (signedAngle > Arc.Limits.First().Value && signedAngle < Arc.Limits.Last().Value) return;
                }
            }

            Success();
        }

        private void CheckRays()
        {
            if (IsShotAvailable) return;

            Dictionary<MeshCollider, bool> originalColliderValues = new Dictionary<MeshCollider, bool>();
            foreach (var collider in Board.Objects)
            {
                if (collider == null)
                {
                    Debug.Log("Collider is null, ignore...");
                    continue;
                }

                originalColliderValues.Add(collider, collider.enabled);
                collider.enabled = (collider.tag == "ShipId:" + Ship2.ShipId) ? true : false;
            }

            bool rayIsFound = false;
            foreach (var limit in Arc.Limits)
            {
                Vector3 vectorFromDegrees = new Vector3((float)Math.Sin(limit.Value * Mathf.Deg2Rad), 0, (float)Math.Cos(limit.Value * Mathf.Deg2Rad));
                vectorFromDegrees = Ship1.TransformVector(vectorFromDegrees);

                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(Ship1.ShipBase.GetGlobalPoint(limit.Key) + new Vector3(0, 0.003f, 0), vectorFromDegrees + new Vector3(0, 0.003f, 0), out hitInfo, Board.BoardIntoWorld(3 * Board.RANGE_1)))
                {
                    if (hitInfo.collider.tag == "ShipId:" + Ship2.ShipId)
                    {
                        if (!rayIsFound)
                        {
                            MinDistance = new RangeHolder(Ship1.ShipBase.GetGlobalPoint(limit.Key), hitInfo.point, Ship1, Ship2);
                            rayIsFound = true;
                        }
                        else
                        {
                            RangeHolder secondRayResult = new RangeHolder(Ship1.ShipBase.GetGlobalPoint(limit.Key), hitInfo.point, Ship1, Ship2);
                            if (secondRayResult.DistanceReal < MinDistance.DistanceReal) MinDistance = new RangeHolder(Ship1.ShipBase.GetGlobalPoint(limit.Key), hitInfo.point, Ship1, Ship2);
                        }
                    }
                }
            }

            if (rayIsFound) Success();

            foreach (var collider in Board.Objects)
            {
                collider.enabled = originalColliderValues[collider];
            }
        }

        private void Success()
        {
            IsShotAvailable = true;

            if (Arc.ArcType != ArcType.None)
            {
                InArc = true;
            }
        }
    }
}



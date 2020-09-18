using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Arcs;
using Upgrade;
using Obstacles;

namespace BoardTools
{

    public class ShotInfoArc : GenericShipDistanceInfo
    {
        public bool IsShotAvailable { get; private set; }
        public bool InArc { get; private set; }

        public IShipWeapon Weapon { get; private set; }

        public List<GenericShip> ObstructedByShips { get; private set; }
        public List<GenericObstacle> ObstructedByObstacles { get; private set; }
        public bool IsObstructedByBombToken { get; private set; }

        private Dictionary<Collider, int> IgnoredColliders;

        private GenericArc Arc;

        public ShotInfoArc(GenericShip ship1, GenericShip ship2, GenericArc arc) : base(ship1, ship2)
        {
            Weapon = ship1.PrimaryWeapons.First();

            Arc = arc;

            CheckRange();
        }

        public ShotInfoArc(GenericShip ship1, GenericShip ship2, GenericArc arc, IShipWeapon weapon) : base(ship1, ship2)
        {
            Weapon = weapon ?? ship1.PrimaryWeapons.First();

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

            CheckObstruction();
        }

        private void CheckRequirements()
        {
            if (Arc.CannotBeUsedForAttackThisRound) return;

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

            Dictionary<Collider, bool> originalColliderValues = new Dictionary<Collider, bool>();
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
            if (IsNormalArc() || IsSubArc() || IsWeaponCanAttackFromBullseyeArc())
            {
                IsShotAvailable = true;
            }

            if (Arc.ArcType != ArcType.None && Ship1 != Ship2)
            {
                InArc = true;
            }
        }

        private bool IsNormalArc()
        {
            return Arc.ArcType != ArcType.Bullseye;
        }

        private bool IsSubArc()
        {
            return Arc.ArcType == ArcType.Bullseye
                && (Ship1.ArcsInfo.Arcs.Any(a => a.Facing == ArcFacing.Front)
                    || Ship1.ArcsInfo.Arcs.Any(a => a.Facing == ArcFacing.FullFront)
                );
        }

        private bool IsWeaponCanAttackFromBullseyeArc()
        {
            return Arc.ArcType == ArcType.Bullseye
                && Weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Bullseye);
        }

        private void CheckObstruction()
        {
            ObstructedByShips = new List<GenericShip>();
            ObstructedByObstacles = new List<GenericObstacle>();
            IgnoredColliders = new Dictionary<Collider, int>();

            CheckObstructionRecursive(MinDistance.Point1, MinDistance.Point2, Ship2);
            RestoreColliders();

            CheckObstructionRecursive(MinDistance.Point2, MinDistance.Point1, Ship1);
            RestoreColliders();
        }

        private void RestoreColliders()
        {
            foreach (var collider in IgnoredColliders)
            {
                collider.Key.gameObject.layer = collider.Value;
            }
        }

        private void CheckObstructionRecursive(Vector3 point1, Vector3 point2, GenericShip targetShip)
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(point1 + new Vector3(0, 0.003f, 0), point2 - point1 + new Vector3(0, 0.003f, 0), out hitInfo))
            {
                if (hitInfo.collider.tag == "Obstacle")
                {
                    GenericObstacle obstructedObstacle = ObstaclesManager.GetChosenObstacle(hitInfo.collider.name);
                    if (!ObstructedByObstacles.Contains(obstructedObstacle)) ObstructedByObstacles.Add(obstructedObstacle);
                    IgnoreCollider(hitInfo);
                    CheckObstructionRecursive(point1, point2, targetShip);
                }
                else if (hitInfo.collider.tag.StartsWith("ShipId:"))
                {
                    if (hitInfo.collider.tag != "ShipId:" + targetShip.ShipId)
                    {
                        GenericShip obstructedShip = Roster.GetShipById(hitInfo.collider.tag);
                        if (!ObstructedByShips.Contains(obstructedShip)) ObstructedByShips.Add(obstructedShip);
                        IgnoreCollider(hitInfo);
                        CheckObstructionRecursive(point1, point2, targetShip);
                    }
                }
                else if (hitInfo.collider.tag == "Mine" || hitInfo.collider.tag == "TimedBomb")
                {
                    IsObstructedByBombToken = true;
                }
            }
        }

        private void IgnoreCollider(RaycastHit hitInfo)
        {
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            if (!IgnoredColliders.ContainsKey(hitInfo.collider)) IgnoredColliders.Add(hitInfo.collider, hitInfo.collider.gameObject.layer);
            hitInfo.collider.gameObject.layer = ignoreRaycastLayer;
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public enum BaseArcsType
    {
        ArcDefault,
        ArcRear,
        Arc180,
        Arc360,
        ArcMobile,
        ArcBullseye,
        ArcGhost
    }

    public class ArcShotPermissions
    {
        public bool CanShootPrimaryWeapon = true;
        public bool CanShootTurret = true;
        public bool CanShootTorpedoes = false;
        public bool CanShootMissiles = false;
        public bool CanShootCannon = false;
    }

    public class ArcInfo
    {
        public GenericShipBase ShipBase;
        public float MinAngle;
        public float MaxAngle;
        public ArcFacing Facing;
        public bool IsMobileArc;
		public bool IsRearAuxArc;

        public ArcShotPermissions ShotPermissions = new ArcShotPermissions();

        public virtual Dictionary<string, Vector3> GetArcPoints()
        {
            Dictionary<string, Vector3> results = new Dictionary<string, Vector3>();

            switch (Facing)
            {
                case ArcFacing.Front:
                    results = ShipBase.GetStandFrontPoints();
                    break;
                case ArcFacing.Left:
                    results = ShipBase.GetStandLeftPoints();
                    break;
                case ArcFacing.Right:
                    results = ShipBase.GetStandRightPoints();
                    break;
                case ArcFacing.Rear:
                    results = ShipBase.GetStandBackPoints();
                    break;
                case ArcFacing.Forward180:
                    results = ShipBase.GetStandFront180Points();
                    break;
                case ArcFacing.Bullseye:
                    results = ShipBase.GetStandBullseyePoints();
                    break;
                default:
                    break;
            }

            return results;
        }
    }

    public enum ArcFacing
    {
        Front,
        Left,
        Right,
        Rear,
        Forward180,
        Bullseye
    }

    public class GenericArc
    {
        public GenericShip Host;

        protected readonly ArcInfo primaryArc;
        protected List<ArcInfo> ArcsList;

        public ArcShotPermissions OutOfArcShotPermissions = new ArcShotPermissions() { CanShootPrimaryWeapon = false };

        public GenericArc(GenericShip host)
        {
            Host = host;

            primaryArc = new ArcInfo()
            {
                ShipBase = Host.ShipBase,
                MinAngle = -40f,
                MaxAngle = 40f,
                Facing = ArcFacing.Front,
                ShotPermissions = new ArcShotPermissions()
                {
                    CanShootPrimaryWeapon = true,
                    CanShootTorpedoes = true,
                    CanShootMissiles = true,
                    CanShootCannon = true,
                    CanShootTurret = true
                }
            };

            ArcsList = new List<ArcInfo>
            {
                primaryArc
            };
        }

        public ArcInfo GetPrimaryArc()
        {
            return ArcsList[0];
        }

        public ArcInfo GetRearArc()
        {
            return ArcsList.Find(n => n.Facing == ArcFacing.Rear);
        }

        public List<ArcInfo> GetAllArcs()
        {
            return ArcsList;
        }

        public virtual bool InAttackAngle(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList);
        }

        public virtual bool InArc(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList);
        }

        public virtual bool InPrimaryArc(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, new List<ArcInfo>() { primaryArc });
        }

        public virtual bool CanShootPrimaryWeapon(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList.Where(n => n.ShotPermissions.CanShootPrimaryWeapon).ToList());
        }

        public virtual bool CanShootTorpedoes(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList.Where(n => n.ShotPermissions.CanShootTorpedoes).ToList());
        }

        public virtual bool CanShootMissiles(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList.Where(n => n.ShotPermissions.CanShootMissiles).ToList());
        }

        public virtual bool CanShootCannon(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList.Where(n => n.ShotPermissions.CanShootCannon).ToList());
        }

        public virtual bool CanShootTurret(string originPoint, float angle)
        {
            return CheckRay(originPoint, angle, ArcsList.Where(n => n.ShotPermissions.CanShootTurret).ToList());
        }

        public virtual bool InBullseyeArc(string originPoint, float angle)
        {
            bool result = false;
            foreach (var arc in ArcsList)
            {
                if (arc.Facing == ArcFacing.Bullseye)
                {
                    result = CheckRay(originPoint, angle, new List<ArcInfo>() { arc });
                }
            }
            return result;
        }

		public virtual bool InRearAuxArc(string originPoint, float angle)
		{
			bool result = false;
			foreach (var arc in ArcsList)
			{
				if (arc.IsRearAuxArc)
				{
					result = CheckRay(originPoint, angle, new List<ArcInfo>() { arc });
				}
			}
			return result;
		}

        public virtual bool InMobileArc(string originPoint, float angle)
        {
            bool result = false;
            foreach (var arc in ArcsList)
            {
                if (arc.IsMobileArc)
                {
                    result = CheckRay(originPoint, angle, new List<ArcInfo>() { arc });
                }
            }
            return result;
        }

        private bool CheckRay(string originPoint, float angle, List<ArcInfo> arcList)
        {

            foreach (var arcInfo in arcList)
            {
                if (!arcInfo.GetArcPoints().ContainsKey(originPoint)) continue;

                if (arcInfo.Facing != ArcFacing.Rear)
                {
                    if (angle >= arcInfo.MinAngle && angle <= arcInfo.MaxAngle)
                    {
                        return true;
                    }
                }
                else
                {
                    if (angle <= arcInfo.MinAngle || angle >= arcInfo.MaxAngle)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual Dictionary<string, Vector3> GetArcsPoints()
        {
            Dictionary<string, Vector3> result = new Dictionary<string, Vector3>();
            foreach (var arc in ArcsList)
            {
                foreach (var point in arc.GetArcPoints())
                {
                    if (!result.ContainsKey(point.Key))
                    {
                        result.Add(point.Key, point.Value);
                    }
                }
            }
            return result;
        }

    }
}

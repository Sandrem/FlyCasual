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
        public bool IsShotAvailable { get; private set; }
        public bool InArc { get { return InArcInfo.Any(n => n.Value == true); } }
        public bool InPrimaryArc { get { return InArcByType(ArcTypes.Primary); } }

        private Dictionary<ArcTypes, bool> InArcInfo { get; set; }

        public bool IsObstructedByAsteroid { get; private set; }
        public bool IsObstructedByBombToken { get; private set; }

        private IShipWeapon Weapon;

        public float DistanceReal { get { return MinDistance.DistanceReal; } }

        public new int Range
        {
            get
            {
                int range = (MinDistance != null) ? MinDistance.Range : int.MaxValue;
                if (OnRangeIsMeasured != null) OnRangeIsMeasured(Ship1, Ship2, Weapon, ref range);
                return range;
            }
        }

        //EVENTS
        public delegate void EventHandlerShipShipWeaponInt(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range);
        public static event EventHandlerShipShipWeaponInt OnRangeIsMeasured;

        public ShotInfo(GenericShip ship1, GenericShip ship2, IShipWeapon weapon = null) : base(ship1, ship2)
        {
            Weapon = weapon ?? ship1.PrimaryWeapon;

            CheckRange();
        }

        private void CheckRange()
        {
            InArcInfo = new Dictionary<ArcTypes, bool>();

            foreach (var arc in Ship1.ArcInfo.Arcs)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(Ship1, Ship2, arc);
                InArcInfo.Add(arc.ArcType, shotInfoArc.InArc);

                WeaponTypes weaponType = (Weapon is GenericSecondaryWeapon) ? (Weapon as GenericSecondaryWeapon).WeaponType : WeaponTypes.PrimaryWeapon;

                if (arc.ShotPermissions.CanShootByWeaponType(weaponType) && shotInfoArc.IsShotAvailable)
                {
                    if (IsShotAvailable == false)
                    {
                        MinDistance = shotInfoArc.MinDistance;
                    }
                    else
                    {
                        if (shotInfoArc.MinDistance.DistanceReal < MinDistance.DistanceReal) MinDistance = shotInfoArc.MinDistance;
                    }

                    IsShotAvailable = true;
                }
            }
        }

        public bool InArcByType(ArcTypes arcType)
        {
            if (!InArcInfo.ContainsKey(arcType)) return false;

            return InArcInfo[arcType];
        }

        public void CheckObstruction(Action callback)
        {
            // Check obstruction

            callback();
        }

        public bool CanShootByWeaponType(WeaponTypes weaponType)
        {
            return true;
        }
    }
}



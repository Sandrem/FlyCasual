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
        public bool InArc { get { return InArcByType.Any(n => n.Value == true); } }
        public Dictionary<ArcTypes, bool> InArcByType { get; private set; }

        public bool ObstructedByAsteroid { get; private set; }
        public bool ObstructedByMine { get; private set; }

        private IShipWeapon Weapon;

        public ShotInfo(GenericShip ship1, GenericShip ship2, IShipWeapon weapon) : base(ship1, ship2)
        {
            Weapon = weapon;

            CheckRange();
        }

        private void CheckRange()
        {
            InArcByType = new Dictionary<ArcTypes, bool>();

            foreach (var arc in Ship1.ArcInfo.Arcs)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(Ship1, Ship2, arc);
                InArcByType.Add(arc.ArcType, shotInfoArc.InArc);

                if (shotInfoArc.InShotAngle)
                {
                    if (InShotAngle == false)
                    {
                        MinDistance = shotInfoArc.MinDistance;
                    }
                    else
                    {
                        if (shotInfoArc.MinDistance.Distance < MinDistance.Distance) MinDistance = shotInfoArc.MinDistance;
                    }

                    InShotAngle = true;
                }
            }
        }
    }
}



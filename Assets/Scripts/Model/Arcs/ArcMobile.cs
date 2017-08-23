using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{

    public class ArcMobile : GenericArc
    {
        private ArcFacing mobileArcFacing = ArcFacing.Forward;
        public ArcFacing MobileArcFacing
        {
            get
            {
                return mobileArcFacing;
            }
            set
            {
                mobileArcFacing = value;
                attackAngles[1] = mobileArcFacings[value];
            }
        }

        private Dictionary<ArcFacing, ArcInfo> mobileArcFacings = new Dictionary<ArcFacing, ArcInfo>
        {
            { ArcFacing.Forward,    new ArcInfo( -40f,  40f) },
            { ArcFacing.Left,       new ArcInfo(-140f, -40f) },
            { ArcFacing.Right,      new ArcInfo(  40f, 140f) },
            { ArcFacing.Rear,       new ArcInfo(-140f, 140f) }
        };

        public ArcMobile()
        {
            attackAngles = new List<ArcInfo> { new ArcInfo(-40f, 40f), mobileArcFacings[ArcFacing.Forward] };
        }
    }
}

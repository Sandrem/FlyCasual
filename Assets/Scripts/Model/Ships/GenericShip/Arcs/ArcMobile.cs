using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{

    public class ArcMobile : GenericArc
    {
        private Transform MobileArcPointer;

        private static Dictionary<ArcFacing, float> MobileArcRotationValues = new Dictionary<ArcFacing, float>
        {
            { ArcFacing.Front, 0f },
            { ArcFacing.Right, 90f },
            { ArcFacing.Rear, 180f },
            { ArcFacing.Left, 270f }
        };

        private ArcFacing mobileArcFacing = ArcFacing.Front;
        public ArcFacing MobileArcFacing
        {
            get
            {
                return mobileArcFacing;
            }
            private set
            {
                mobileArcFacing = value;
                ArcsList[1] = mobileArcFacings[value];
            }
        }

        private Dictionary<ArcFacing, ArcInfo> mobileArcFacings;

        public ArcMobile(GenericShip host) : base(host)
        {
            mobileArcFacings = new Dictionary<ArcFacing, ArcInfo>(){
                {
                    ArcFacing.Front,
                    new ArcInfo()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = -40f,
                        MaxAngle = 40f,
                        Facing = ArcFacing.Front
                    }
                },
                {
                    ArcFacing.Left,
                    new ArcInfo()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = -140f,
                        MaxAngle = -40f,
                        Facing = ArcFacing.Left
                    }
                },
                {
                    ArcFacing.Right,
                    new ArcInfo()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = 40f,
                        MaxAngle = 140f,
                        Facing = ArcFacing.Right
                    }
                },
                {
                    ArcFacing.Rear,
                    new ArcInfo()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = -140f,
                        MaxAngle = 140f,
                        Facing = ArcFacing.Rear
                    }
                }
            };

            ArcsList = new List<ArcInfo>
            {
                primaryArc,
                mobileArcFacings[ArcFacing.Front]
            };

            ShowMobileArcPointer();
        }

        public void ShowMobileArcPointer()
        {
            MobileArcPointer = Host.GetShipAllPartsTransform().Find("ShipBase").Find("MobileArcPointer");
            MobileArcPointer.gameObject.SetActive(true);
        }

        public void RotateArc(ArcFacing facing)
        {
            MobileArcFacing = facing;
            MobileArcPointer.localEulerAngles = new Vector3(0f, MobileArcRotationValues[facing], 0f);
        }
    }
}

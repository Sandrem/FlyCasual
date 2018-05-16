using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;
using ActionsList;

namespace Arcs
{

    public class ArcMobile : GenericArc
    {
        public ArcMobile(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Mobile;
            Facing = ArcFacing.Front;
            MinAngle = -40f;
            MaxAngle = 40f;
            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
                CanShootTurret = true
            };
        }

        public void RotateArc(ArcFacing facing)
        {
            // Rotate Arc
        }

        /*private Transform MobileArcPointer;

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

        private Dictionary<ArcFacing, GenericArc> mobileArcFacings;

        public ArcMobile(GenericShip host) : base(host)
        {
            mobileArcFacings = new Dictionary<ArcFacing, GenericArc>(){
                {
                    ArcFacing.Front,
                    new GenericArc()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = -40f,
                        MaxAngle = 40f,
                        Facing = ArcFacing.Front,
                        IsMobileArc = true
                    }
                },
                {
                    ArcFacing.Left,
                    new GenericArc()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = 40f,
                        MaxAngle = 140f,
                        Facing = ArcFacing.Left,
                        IsMobileArc = true
                    }
                },
                {
                    ArcFacing.Right,
                    new GenericArc()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = -140f,
                        MaxAngle = -40f,
                        Facing = ArcFacing.Right,
                        IsMobileArc = true
                    }
                },
                {
                    ArcFacing.Rear,
                    new GenericArc()
                    {
                        ShipBase = Host.ShipBase,
                        MinAngle = -140f,
                        MaxAngle = 140f,
                        Facing = ArcFacing.Rear,
                        IsMobileArc = true
                    }
                }
            };

            ArcsList = new List<GenericArc>
            {
                primaryArc,
                mobileArcFacings[ArcFacing.Front]
            };

            SubscribeToShipSetup(host);

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
            RuleSets.RuleSet.Instance.RotateMobileFiringArc(facing);
        }

        private void SubscribeToShipSetup(GenericShip host)
        {
            host.OnShipIsPlaced += AskForMobileFiringArcDirection;
        }

        private void AskForMobileFiringArcDirection(GenericShip host)
        {
            host.OnShipIsPlaced -= AskForMobileFiringArcDirection;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Mobile Firing Arc direction",
                TriggerType = TriggerTypes.OnShipIsPlaced,
                TriggerOwner = host.Owner.PlayerNo,
                EventHandler = PerformFreeRotateActionForced
            });
        }

        private void PerformFreeRotateActionForced(object sender, EventArgs e)
        {
            Selection.ThisShip.AskPerformFreeAction(new List<GenericAction>() { new RotateArcAction() }, Triggers.FinishTrigger, true);
        }*/
    }
}

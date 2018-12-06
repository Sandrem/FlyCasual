﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;
using ActionsList;

namespace Arcs
{

    public class ArcDualTurretB : GenericArc
    {
        private Transform MobileArcPointer;

        GenericShip Host;

        private class MobileSubArc
        {
            public ArcFacing Facing { get; private set; }
            public Dictionary<Vector3, float> Limits { get; private set; }
            public List<Vector3> Edges { get; private set; }

            public MobileSubArc(ArcFacing facing, Dictionary<Vector3, float> limits, List<Vector3> edges)
            {
                Facing = facing;
                Limits = limits;
                Edges = edges;
            }
        }

        private static Dictionary<ArcFacing, float> MobileArcRotationValues = new Dictionary<ArcFacing, float>
        {
            { ArcFacing.Front, 0f },
            { ArcFacing.Right, 90f },
            { ArcFacing.Rear, 180f },
            { ArcFacing.Left, 270f }
        };

        private List<MobileSubArc> MobileArcParameters;
        private MobileSubArc ActiveMobileSubArc;

        public override ArcFacing Facing
        {
            get { return ActiveMobileSubArc.Facing; }
            set { ActiveMobileSubArc = MobileArcParameters.Find(n => n.Facing == value); }
        }

        public override List<Vector3> Edges { get { return ActiveMobileSubArc.Edges; } }
        public override Dictionary<Vector3, float> Limits { get { return ActiveMobileSubArc.Limits; } }

        public ArcDualTurretB(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcType.SingleTurret;

            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
                CanShootTurret = true
            };

            // Arcs

            MobileArcParameters = new List<MobileSubArc>
            {
                new MobileSubArc
                (
                    ArcFacing.Front,
                    new Dictionary<Vector3, float>()
                    {
                        { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0), -40f },
                        { new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),  40f }
                    },
                    new List<Vector3>()
                    {
                        new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                        new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                    }
                ),
                new MobileSubArc
                (
                    ArcFacing.Left,
                    new Dictionary<Vector3, float>()
                    {
                        { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE), -140f },
                        { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),  -40f }
                    },
                    new List<Vector3>()
                    {
                        new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                        new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                        new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                        new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                    }
                ),
                new MobileSubArc
                (
                    ArcFacing.Right,
                    new Dictionary<Vector3, float>()
                    {
                        { new Vector3(shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),  40f },
                        { new Vector3(shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE), 140f },
                    },
                    new List<Vector3>()
                    {
                        new Vector3(shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                        new Vector3(shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                        new Vector3(shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                        new Vector3(shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                    }
                ),
                new MobileSubArc
                (
                    ArcFacing.Rear,
                    new Dictionary<Vector3, float>()
                    {
                        { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE), -140f },
                        { new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),  140f }
                    },
                    new List<Vector3>()
                    {
                        new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                        new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                    }
                )
            };

            ActiveMobileSubArc = MobileArcParameters[0];

            // Events
            Host = shipBase.Host;
            // SubscribeToShipSetup(Host); - Done be A-arc

            // Pointer
            ShowMobileArcPointer();
        }

        public void RotateArc(ArcFacing facing)
        {
            Facing = facing;
            MobileArcPointer.localEulerAngles = new Vector3(0f, MobileArcRotationValues[facing], 0f);
            RuleSets.Edition.Current.RotateMobileFiringArcAlt(facing);
        }

        public void ShowMobileArcPointer()
        {
            MobileArcPointer = Host.GetShipAllPartsTransform().Find("ShipBase").Find("MobileArcPointerAlt");
            MobileArcPointer.gameObject.SetActive(true);
            MobileArcPointer.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }
}

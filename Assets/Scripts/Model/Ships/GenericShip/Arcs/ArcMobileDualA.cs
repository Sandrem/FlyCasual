using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;
using ActionsList;

namespace Arcs
{

    public class ArcMobileDualA : GenericArc
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
            { ArcFacing.Forward, 0f },
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

        public ArcMobileDualA(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Mobile;

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
                    ArcFacing.Forward,
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
            SubscribeToShipSetup(Host);

            // Pointer
            ShowMobileArcPointer();
        }

        public void RotateArc(ArcFacing facing)
        {
            Facing = facing;
            switch (facing)
            {
                case ArcFacing.Forward:
                    MobileArcPointer.localEulerAngles = new Vector3(0f, MobileArcRotationValues[ArcFacing.Forward], 0f);
                    (Host.ArcInfo.Arcs.Find(a => a is ArcMobileDualB) as ArcMobileDualB).RotateArc(ArcFacing.Rear);
                    break;
                case ArcFacing.Left:
                    MobileArcPointer.localEulerAngles = new Vector3(0f, MobileArcRotationValues[ArcFacing.Left], 0f);
                    (Host.ArcInfo.Arcs.Find(a => a is ArcMobileDualB) as ArcMobileDualB).RotateArc(ArcFacing.Right);
                    break;
            }
            RuleSets.RuleSet.Instance.RotateMobileFiringArc(facing);
        }

        public void ShowMobileArcPointer()
        {
            MobileArcPointer = Host.GetShipAllPartsTransform().Find("ShipBase").Find("MobileArcPointer");
            MobileArcPointer.gameObject.SetActive(true);
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
            Selection.ThisShip.AskPerformFreeAction(
                new List<GenericAction>() { new RotateArcAction() },
                delegate{
                    Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(RotateArcAction));
                    Triggers.FinishTrigger();
                },
                true);
        }
    }
}

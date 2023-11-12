using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class SithInfiltrator : GenericShip
    {
        public SithInfiltrator() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "Sith Infiltrator",
                BaseSize.Large,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Separatists, typeof(DarthMaulPilot) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 3), 1, 6, 4,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red)
                ),
                new ShipUpgradesInfo()
            );

            ModelInfo = new ShipModelInfo
            (
                "Sith Infiltrator",
                "Default",
                new Vector3(-4.17f, 8.25f, 5.55f),
                2.5f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "Slave1-Fly1",
                    "Slave1-Fly2"
                },
                "Slave1-Fire", 3
            );

            ShipIconLetter = ']';
        }
    }
}

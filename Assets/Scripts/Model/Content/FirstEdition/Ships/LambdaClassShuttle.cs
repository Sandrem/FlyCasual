﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.LambdaClassShuttle
    {
        public class LambdaClassShuttle : GenericShip
        {
            public LambdaClassShuttle() : base()
            {
                ShipInfo = new ShipCardInfo
                (
                    "Lambda-class Shuttle",
                    BaseSize.Large,
                    Faction.Imperial,
                    new ShipArcsInfo(ArcType.Front, 3), 1, 5, 5,
                    new ShipActionsInfo(
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction))
                    ),
                    new ShipUpgradesInfo(
                        UpgradeType.Title,
                        UpgradeType.System,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Crew,
                        UpgradeType.Crew
                    )
                );

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Imperial, typeof(CaptainYorr) }
                };

                ModelInfo = new ShipModelInfo(
                    "Lambda-class Shuttle",
                    "Default"
                );

                DialInfo = new ShipDialInfo(
                    new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo(
                    new List<string>()
                    {
                        "Slave1-Fly1",
                        "Slave1-Fly2"
                    },
                    "Slave1-Fire", 3
                );

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/c/c3/MI_LAMBDA-SHUTTLE.png";

                HotacManeuverTable = new AI.LambdaShuttleTable();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.XiClassLightShuttle
{
    public class XiClassLightShuttle : GenericShip
    {
        public XiClassLightShuttle() : base()
        {
            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ShipInfo = new ShipCardInfo
            (
                "Xi-class Light Shuttle",
                BaseSize.Medium,
                Faction.FirstOrder,
                new ShipArcsInfo(ArcType.Front, 2), 2, 5, 2,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction), ActionColor.Red),
                    new ActionInfo(typeof(CoordinateAction), ActionColor.Red),
                    new ActionInfo(typeof(JamAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Crew,
                    UpgradeType.Illicit,
                    UpgradeType.Modification
                )
            );

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.FirstOrder, typeof(GideonHask) }
            };

            ModelInfo = new ShipModelInfo(
                "Xi-Class Light Shuttle",
                "Gray",
                new Vector3(-3.73f, 8.65f, 5.55f),
                2.3f
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "Slave1-Fly1",
                    "Slave1-Fly2"
                },
                "Slave1-Fire", 4
            );
        }
    }
}
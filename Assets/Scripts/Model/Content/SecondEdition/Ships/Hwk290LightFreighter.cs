﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Arcs;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class Hwk290LightFreighter : FirstEdition.Hwk290.Hwk290
        {
            public Hwk290LightFreighter() : base()
            {
                ShipInfo.ShipName = "HWK-290 Light Freighter";

                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.SingleTurret, 2);
                ShipInfo.Hull = 3;
                ShipInfo.Shields = 2;

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Turret);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Bomb);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Modification);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), ActionColor.Red));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction), ActionColor.Red));

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(RotateArcAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(TargetLockAction), typeof(RotateArcAction)));

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);

                IconicPilots[Faction.Rebel] = typeof(RebelScout);
                IconicPilots[Faction.Scum] = typeof(PalobGodalhi);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/8/8a/Maneuver_hwk-290.png";

                OldShipTypeName = "HWK-290";
            }
        }
    }
}

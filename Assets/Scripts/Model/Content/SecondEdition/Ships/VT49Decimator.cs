﻿using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Upgrade;

namespace Ship.SecondEdition.VT49Decimator
{
    public class VT49Decimator : FirstEdition.VT49Decimator.VT49Decimator
    {
        public VT49Decimator() : base()
        {
            ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.DoubleTurret, 3);

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Crew);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction), ActionColor.Red));

            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Complex);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

            IconicPilots[Faction.Imperial] = typeof(CaptainOicunn);

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/9/9e/Maneuver_vt-49_decimator.png";
        }
    }
}

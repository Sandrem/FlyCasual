﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;
using Arcs;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class JumpMaster5000 : FirstEdition.JumpMaster5000.JumpMaster5000
        {
            public JumpMaster5000() : base()
            {
                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.SingleTurret, 2);
                ShipInfo.Hull = 6;
                ShipInfo.Shields = 3;

                ShipInfo.ActionIcons.RemoveActions(typeof(BarrelRollAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(RotateArcAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(TargetLockAction), typeof(RotateArcAction)));

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Torpedo);

                IconicPilots[Faction.Scum] = typeof(Dengar);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop));
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop));

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/9/9f/Maneuver_jumpmaster.png";
            }
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class ASF01BWing : FirstEdition.BWing.BWing
        {
            public ASF01BWing() : base()
            {
                ShipInfo.ShipName = "A/SF-01 B-wing";
                ShipInfo.Hull = 4;
                ShipInfo.Shields = 4;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Cannon);
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(BarrelRollAction)));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);

                IconicPilots[Faction.Rebel] = typeof(BraylenStramm);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/f/ff/Maneuver_b-wing.png";

                OldShipTypeName = "B-wing";
            }
        }
    }
}

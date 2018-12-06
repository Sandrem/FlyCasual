using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Movement;
using Upgrade;

namespace Ship.SecondEdition.ModifiedYT1300LightFreighter
{
    public class ModifiedYT1300LightFreighter : FirstEdition.YT1300.YT1300
    {
        public ModifiedYT1300LightFreighter() : base()
        {
            ShipInfo.ShipName = "Modified YT-1300 Light Freighter";
            ShipInfo.ArcInfo.Firepower = 3;
            ShipInfo.Hull = 8;
            ShipInfo.Shields = 5;

            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Missile);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), ActionColor.Red));

            DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn));
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn));
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Normal);
            DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);

            IconicPilots[Faction.Rebel] = typeof(HanSolo);

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/3f/Maneuver_modified_yt-1300.png";
        }
    }
}

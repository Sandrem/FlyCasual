using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class BTLA4YWing : FirstEdition.YWing.YWing
        {
            public BTLA4YWing() : base()
            {
                ShipInfo.ShipName = "BTL-A4 Y-wing";
                ShipInfo.Hull = 6;
                ShipInfo.Shields = 2;

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction), ActionColor.Red));

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Bomb);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);

                IconicPilots[Faction.Rebel] = typeof(NorraWexley);
                IconicPilots[Faction.Scum] = typeof(DreaRenthal);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/18/Maneuver_y-wing.png";
            }
        }
    }
}

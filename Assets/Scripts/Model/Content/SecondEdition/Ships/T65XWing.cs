using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class T65XWing : FirstEdition.XWing.XWing, IMovableWings
        {
            public T65XWing() : base()
            {
                ShipInfo.ShipName = "T-65 X-wing";
                ShipInfo.Hull = 4;
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Configuration);
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll), MovementComplexity.Complex);

                IconicPilots[Faction.Rebel] = typeof(LukeSkywalker);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/cf/Maneuver_t-65_x-wing.png";
            }
        }
    }
}

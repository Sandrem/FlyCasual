using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class AuzituckGunship : FirstEdition.AuzituckGunship.AuzituckGunship
        {
            public AuzituckGunship() : base()
            {
                ShipInfo.Shields = 2;

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight));

                IconicPilots[Faction.Rebel] = typeof(Lowhhrick);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/db/Maneuver_auzituck.png";
            }
        }
    }
}

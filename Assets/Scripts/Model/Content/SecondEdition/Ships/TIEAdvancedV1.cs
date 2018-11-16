using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class TIEAdvancedV1 : FirstEdition.TIEAdvPrototype.TIEAdvPrototype, TIE
        {
            public TIEAdvancedV1() : base()
            {
                ShipInfo.ShipName = "TIE Advanced v1";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.System);
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(EvadeAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), typeof(FocusAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));

                IconicPilots[Faction.Imperial] = typeof(GrandInquisitor);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
            }
        }
    }
}

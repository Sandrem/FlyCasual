using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class StarViperClassAttackPlatform : FirstEdition.StarViper.StarViper
        {
            public StarViperClassAttackPlatform() : base()
            {
                ShipInfo.ShipName = "StarViper-class Attack Platform";

                IconicPilots[Faction.Scum] = typeof(BlackSunEnforcer);

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.System);

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(FocusAction)));

                ShipAbilities.Add(new Abilities.FirstEdition.StarViperMkIIAbility());

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/e1/Maneuver_starviper.png";

                OldShipTypeName = "StarViper";
            }
        }
    }
}
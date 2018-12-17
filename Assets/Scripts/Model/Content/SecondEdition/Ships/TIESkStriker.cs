using System.Collections;
using System.Collections.Generic;
using Movement;
using Upgrade;
using Ship;
using SubPhases;
using System;
using GameModes;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class TIESkStriker : FirstEdition.TIEStriker.TIEStriker, TIE
        {
            public TIESkStriker() : base()
            {
                ShipInfo.ShipName = "TIE/sk Striker";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Bomb);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

                ShipAbilities.Add(new Abilities.FirstEdition.AdaptiveAileronsAbility());

                IconicPilots[Faction.Imperial] = typeof(Duchess);

                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/43/Maneuver_tie_striker.png";

                OldShipTypeName = "TIE Striker";
            }
        }
    }
}
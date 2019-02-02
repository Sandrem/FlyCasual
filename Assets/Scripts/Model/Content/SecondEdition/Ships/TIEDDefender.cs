using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;
using Ship;
using System;
using SubPhases;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class TIEDDefender : FirstEdition.TIEDefender.TIEDefender, TIE
        {
            public TIEDDefender() : base()
            {
                ShipInfo.ShipName = "TIE/D Defender";

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.System);

                ShipInfo.Shields = 4;

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(EvadeAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction)));

                ShipAbilities.Add(new Abilities.FirstEdition.TIEx7Ability());

                IconicPilots[Faction.Imperial] = typeof(CountessRyad);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/18/Maneuver_tie_defender.png";

                OldShipTypeName = "TIE Defender";
            }
        }
    }
}